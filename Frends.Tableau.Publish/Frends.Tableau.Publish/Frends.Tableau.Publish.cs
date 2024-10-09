namespace Frends.Tableau.Publish;

using Frends.Tableau.Publish.Definitions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Xml.Linq;

/// <summary>
/// Tableau Task.
/// </summary>
public static class Tableau
{
    private static readonly ObjectCache ClientCache = MemoryCache.Default;
    private static readonly CacheItemPolicy CachePolicy = new() { SlidingExpiration = TimeSpan.FromHours(1) };
    private static readonly string CacheKey = "DefaultHttpClient";
    private static string baseUrl = string.Empty;
    private static string siteId = string.Empty;

    /// <summary>
    /// This is Task.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.Tableau.Publish).
    /// </summary>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="input">Input parameters.</param>
    /// <param name="options">Options parameters.</param>
    /// <param name="cancellationToken">Cancellation token given by Frends.</param>
    /// <returns>Object { string Output }.</returns>
    public static Result Publish([PropertyTab] Connection connection, [PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        baseUrl = @$"{connection.BaseUrl}".TrimEnd('/');
        var httpClient = GetHttpClient(connection, cancellationToken);

        var projectId = input.ProjectId;
        if (string.IsNullOrEmpty(projectId))
            projectId = GetOrCreateProject(httpClient, input, options, cancellationToken);

        if (input.SingleRequest)
            return new Result([.. PublishWorkbook(httpClient, input, projectId, cancellationToken)]);
        else
            return new Result([.. PublishMultipartsWorkbook(httpClient, input, projectId)]);
    }

    private static HttpClient GetHttpClient(Connection connection, CancellationToken cancellationToken)
    {
        if (ClientCache.Get(CacheKey) is HttpClient httpClient)
            return httpClient;

        httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.ExpectContinue = true;

        if (!string.IsNullOrEmpty(connection.CredentialsToken))
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Tableau-Auth", connection.CredentialsToken);
            siteId = connection.SiteId;
        }
        else
        {
            var signInRequest = SignIn(httpClient, connection, cancellationToken);

            if (!signInRequest.IsSuccessStatusCode)
                throw new Exception(signInRequest.Content.ReadAsStringAsync(cancellationToken).Result);

            var extractedData = ExtractXMLData(signInRequest.Content.ReadAsStringAsync(cancellationToken).Result);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Tableau-Auth", extractedData.TryGetValue("token", out var t) ? t : null);
            siteId = extractedData.TryGetValue("siteId", out var si) ? si : null;
        }

        ClientCache.Add(CacheKey, httpClient, CachePolicy);
        return httpClient;
    }

    private static HttpResponseMessage SignIn(HttpClient client, Connection connection, CancellationToken cancellationToken)
    {
        string json;

        if (connection.AuthenticationType is AuthenticationTypes.Basic && !string.IsNullOrEmpty(connection.UserName) && !string.IsNullOrEmpty(connection.Password))
        {
            json = $@"
                {{
                    ""credentials"": {{
                        ""name"": ""{connection.UserName}"",
                        ""password"": ""{connection.Password}"",
                        ""site"": {{
                            ""contentUrl"": ""{connection.SiteName}""
                        }}
                    }}
                }}";
        }
        else if (connection.AuthenticationType is AuthenticationTypes.PAT && !string.IsNullOrEmpty(connection.PatName) && !string.IsNullOrEmpty(connection.PatSecret))
        {
            json = $@"
                {{
                    ""credentials"": {{
                        ""personalAccessTokenName"": ""{connection.PatName}"",
                        ""personalAccessTokenSecret"": ""{connection.PatSecret}"",
                        ""site"": {{
                            ""contentUrl"": ""{connection.SiteName}""
                        }}
                    }}
                }}";
        }
        else
        {
            throw new ArgumentException("Missing or invalid authentication values.");
        }

        var result = client.PostAsync($@"{baseUrl}/auth/signin", new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken).Result;

        if (!result.IsSuccessStatusCode)
            throw new Exception(result.Content.ReadAsStringAsync(cancellationToken).Result);

        return result;
    }

    private static string GetOrCreateProject(HttpClient client, Input input, Options options, CancellationToken cancellationToken)
    {
        var getProjectsRequest = client.GetAsync($@"{baseUrl}/sites/{siteId}/projects", cancellationToken).Result;

        if (!getProjectsRequest.IsSuccessStatusCode)
            throw new Exception(getProjectsRequest.Content.ReadAsStringAsync(cancellationToken).Result);

        var extractProjects = ExtractXMLData(getProjectsRequest.Content.ReadAsStringAsync(cancellationToken).Result);

        if (extractProjects.ContainsKey(input.ProjectName))
            return extractProjects.TryGetValue(input.ProjectName, out var pId) ? pId : null;

        if (!extractProjects.ContainsKey(input.ProjectName) && options.CreateProject)
        {
            var json = $@"
                {{
                    ""project"": {{
                        ""name"": ""{input.ProjectName}"",
                        ""description"": ""{input.ProjectDescription}""
                    }}
                }}";

            var createProjectRequest = client.PostAsync($@"{baseUrl}/sites/{siteId}/projects", new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken).Result;

            if (!createProjectRequest.IsSuccessStatusCode)
                throw new Exception(createProjectRequest.Content.ReadAsStringAsync(cancellationToken).Result);

            return ExtractXMLData(createProjectRequest.Content.ReadAsStringAsync(cancellationToken).Result).Values.First();
        }
        else
        {
            throw new Exception($"Project {input.ProjectName} doesn't exist and Options.CreateProject is false.");
        }
    }

    private static List<FileUploadResult> PublishWorkbook(HttpClient client, Input input, string projectId, CancellationToken cancellationToken)
    {
        var results = new List<FileUploadResult>();
        foreach (var file in input.Files)
        {
            using var multipartContent = new MultipartContent("mixed");

            if (!File.Exists(input.Xml))
            {
                if (input.CreateXml)
                    CreateXmlFile(input, projectId);
                else
                    throw new Exception("XML missing.");
            }

            using var xmlFileContent = new StreamContent(new FileStream(input.Xml, FileMode.Open, FileAccess.Read));
            xmlFileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "request_payload" };
            xmlFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");

            multipartContent.Add(xmlFileContent);

            using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var fileStreamContent = new StreamContent(fileStream);
            var contentDispositionName = string.Empty;

            switch (input.ResourceType)
            {
                case ResourceTypes.Workbooks:
                    contentDispositionName = "tableau_workbook";
                    break;
                case ResourceTypes.Flows:
                    contentDispositionName = "tableau_flow";
                    break;
                case ResourceTypes.DataSources:
                    contentDispositionName = "tableau_datasource";
                    break;
                default:
                    break;
            }

            fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = contentDispositionName,
                FileName = Path.GetFileName(file),
            };

            fileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            multipartContent.Add(fileStreamContent);

            var publishRequest = client.PostAsync($"{baseUrl}/sites/{siteId}/{input.ResourceType.ToString().ToLower()}?overwrite={input.Overwrite}", multipartContent, cancellationToken).Result;

            if (!publishRequest.IsSuccessStatusCode)
                throw new Exception(publishRequest.Content.ReadAsStringAsync(cancellationToken).Result);

            results.Add(new FileUploadResult { File = file, Success = true, Message = publishRequest.Content.ReadAsStringAsync(cancellationToken).Result, StatusCode = (int)publishRequest.StatusCode });
        }

        return results;
    }

    private static List<FileUploadResult> PublishMultipartsWorkbook(HttpClient client, Input input, string projectId)
    {
        var results = new List<FileUploadResult>();
        var disposableResources = new List<IDisposable>();

        var initFileUpload = client.PostAsync($@"{baseUrl}/sites/{siteId}/fileUploads", null).Result;
        var initFileUploadContent = initFileUpload.Content.ReadAsStringAsync().Result;
        var exateddata = ExtractXMLData(initFileUploadContent);
        var uploadSessionId = exateddata.TryGetValue("uploadSessionId", out var usi) ? usi : null;

        using var multipartContent = new MultipartContent("mixed");

        foreach (var file in input.Files)
        {
            if (!File.Exists(input.Xml))
            {
                if (input.CreateXml)
                    CreateXmlFile(input, projectId);
                else
                    throw new Exception("XML missing.");
            }

            var xmlFileContent = new StreamContent(new FileStream(input.Xml, FileMode.Open, FileAccess.Read));
            xmlFileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "request_payload" };
            xmlFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");

            multipartContent.Add(xmlFileContent);
            disposableResources.Add(xmlFileContent);

            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var fileStreamContent = new StreamContent(fileStream);

            fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "tableau_file",
                FileName = Path.GetFileName(file),
            };
            fileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            multipartContent.Add(fileStreamContent);
            disposableResources.Add(fileStreamContent);
            disposableResources.Add(fileStream);

            var publishPutRequest = client.PutAsync($"{baseUrl}/sites/{siteId}/fileUploads/{uploadSessionId}", multipartContent).Result;

            var responseContent = publishPutRequest.Content.ReadAsStringAsync().Result;
            var extractedData = ExtractXMLData(responseContent);

            results.Add(new FileUploadResult
            {
                File = file,
                Success = publishPutRequest.IsSuccessStatusCode,
                Message = responseContent,
                StatusCode = (int)publishPutRequest.StatusCode,
                JobId = extractedData.TryGetValue("jobId", out var jobId) ? jobId : string.Empty,
            });
        }

        var requestUrl = $"{baseUrl}/sites/{siteId}/{input.ResourceType.ToString().ToLower()}?uploadSessionId={uploadSessionId}&overwrite={input.Overwrite}";
        var fileType = string.IsNullOrEmpty(input.FileType) ? Path.GetExtension(input.FileType) : input.FileType;

        switch (input.ResourceType)
        {
            case ResourceTypes.Workbooks:
                requestUrl += $"&workbookType={fileType}&asJob={input.AsJob}";
                break;
            case ResourceTypes.Flows:
                requestUrl += $"&flowType={fileType}";
                break;
            case ResourceTypes.DataSources:
                requestUrl += $"&datasourceType={fileType}&append={input.Append}&asJob={input.AsJob}";
                break;
            default:
                break;
        }

        var publishRequest = client.PostAsync(requestUrl, multipartContent).Result;

        if (!publishRequest.IsSuccessStatusCode)
            throw new Exception(publishRequest.Content.ReadAsStringAsync().Result);

        // Dispose of all collected resources
        foreach (var resource in disposableResources)
            resource.Dispose();

        return results;
    }

    private static Dictionary<string, string> ExtractXMLData(string xmlResponse)
    {
        XDocument xmlDoc = XDocument.Parse(xmlResponse);
        XNamespace ns = "http://tableau.com/api";
        var result = new Dictionary<string, string>();

        // creds
        var token = xmlDoc.Root.Element(ns + "credentials")?.Attribute("token")?.Value;
        var siteId = xmlDoc.Root.Element(ns + "credentials")?.Element(ns + "site")?.Attribute("id")?.Value;
        var contentUrl = xmlDoc.Root.Element(ns + "credentials")?.Element(ns + "site")?.Attribute("contentUrl")?.Value;
        var userId = xmlDoc.Root.Element(ns + "credentials")?.Element(ns + "user")?.Attribute("id")?.Value;

        if (token != null) result["token"] = token;
        if (siteId != null) result["siteId"] = siteId;
        if (contentUrl != null) result["contentUrl"] = contentUrl;
        if (userId != null) result["userId"] = userId;

        // projects
        foreach (var project in xmlDoc.Descendants(ns + "project"))
        {
            var id = project.Attribute("id")?.Value;
            var name = project.Attribute("name")?.Value;
            if (id != null && name != null)
            {
                result[name] = id;
            }
        }

        // file session
        var uploadSessionId = xmlDoc.Root.Element(ns + "fileUpload")?.Attribute("uploadSessionId")?.Value;
        var uploadSessionFileSize = xmlDoc.Root.Element(ns + "fileUpload")?.Attribute("fileSize")?.Value;
        if (uploadSessionId != null) result["uploadSessionId"] = uploadSessionId;
        if (uploadSessionFileSize != null) result["fileSize"] = uploadSessionFileSize;

        // job id
        var jobId = xmlDoc.Root.Element(ns + "job")?.Attribute("id")?.Value;
        if (jobId != null) result["jobId"] = jobId;

        return result;
    }

    private static void CreateXmlFile(Input input, string projectId)
    {
        var xml = string.Empty;
        switch (input.ResourceType)
        {
            case ResourceTypes.Workbooks:
                xml = $@"<tsRequest>
  <workbook name=""{input.ResourceName}"">
    <project id=""{projectId}""/>
  </workbook>
</tsRequest>";
                break;
            case ResourceTypes.Flows:
                xml = $@"<tsRequest>
  <flow name=""{input.ResourceName}"">
    <project id=""{projectId}""/>
  </flow>
</tsRequest>";
                break;
            case ResourceTypes.DataSources:
                xml = $@"<tsRequest>
  <datasource name=""{input.ResourceName}"">
    <project id=""{projectId}""/>
  </datasource>
</tsRequest>";
                break;
            default:
                break;
        }

        File.WriteAllText(input.Xml, xml);
    }
}