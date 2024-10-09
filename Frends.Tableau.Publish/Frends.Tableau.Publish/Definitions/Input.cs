namespace Frends.Tableau.Publish.Definitions;

using System.ComponentModel;

/// <summary>
/// Contains the input parameters for the publish operation.
/// </summary>
public class Input
{
    /// <summary>
    /// Indicates whether to publish the file in a single request.
    /// The maximum size of a file that can be published in a single request is 64 MB.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool SingleRequest { get; internal set; }

    /// <summary>
    /// The type of resource to be published.
    /// </summary>
    /// <example>ResourceTypes.Workbooks</example>
    [DefaultValue(ResourceTypes.Workbooks)]
    public ResourceTypes ResourceType { get; set; }

    /// <summary>
    /// The ID of the project where the resource will be published.
    /// Leave empty if not sure.
    /// </summary>
    /// <example>ed36a27e-7507-4695-bc0d-ac2cb060cf11</example>
    public string ProjectId { get; set; }

    /// <summary>
    /// The name of the project.
    /// If the specified project does not exist, a new project with this name will be created.
    /// </summary>
    /// <example>Project1</example>
    public string ProjectName { get; set; }

    /// <summary>
    /// A description of the project.
    /// </summary>
    /// <example>This is a description.</example>
    public string ProjectDescription { get; set; }

    /// <summary>
    /// The name of the resource to be published.
    /// </summary>
    /// <example>NewResource</example>
    public string ResourceName { get; set; }

    /// <summary>
    /// The path to the XML file.
    /// This value will be used to create a new XML file if it does not exist and <see cref="CreateXml"/> is true.
    /// </summary>
    /// <example>c:\\temp\requestxml.xml</example>
    public string Xml { get; set; }

    /// <summary>
    /// Indicates whether to create a request XML file at the path specified in <see cref="Xml"/>.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool CreateXml { get; set; }

    /// <summary>
    /// The full paths to the files to be uploaded.
    /// </summary>
    /// <example>["c:\\temp\file.twbx", "c:\\temp\file2.twbx"]</example>
    public string[] Files { get; set; }

    /// <summary>
    /// The type of file being uploaded.
    /// This will be determined from the file's extension if left empty.
    /// </summary>
    /// <example>twbx</example>
    public string FileType { get; set; }

    /// <summary>
    /// Indicates whether to append the data being published to an existing data source with the same name.
    /// If set to true but the data source does not exist, the operation fails.
    /// Both the source file and the published file must be extracts with the .hyper or .tde extension, and their schemas must match.
    /// Cannot append data to an extract stored using the multiple tables option.
    /// Both the overwrite and append parameters cannot be true simultaneously.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool Append { get; set; }

    /// <summary>
    /// Indicates whether to overwrite an existing workbook with the same name.
    /// If set to true and the workbook does not exist, the operation succeeds.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool Overwrite { get; set; }

    /// <summary>
    /// Indicates whether the publishing process should run asynchronously.
    /// If set to true, the process runs as a background job and returns a job ID.
    /// This can help avoid timeouts for large workbooks and can also help resolve some publishing issues.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool AsJob { get; set; }
}