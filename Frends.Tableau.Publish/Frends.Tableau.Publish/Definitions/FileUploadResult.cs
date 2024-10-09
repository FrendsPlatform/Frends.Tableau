namespace Frends.Tableau.Publish.Definitions;

/// <summary>
/// Represents the result of a file upload operation.
/// </summary>
public class FileUploadResult
{
    /// <summary>
    /// The path of the uploaded file.
    /// </summary>
    /// <example>c:\\temp\file.tflx</example>
    public string File { get; set; }

    /// <summary>
    /// Indicates whether the upload request was successful.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// The message returned from the upload request.
    /// </summary>
    /// <example>"&lt;tsResponse&gt;&lt;workbook id='...' name='...' contentUrl='...' webpageUrl='...' createdAt='...' updatedAt='...'&gt;&lt;/workbook&gt;&lt;/tsResponse&gt;"</example>
    public string Message { get; set; }

    /// <summary>
    /// The HTTP status code returned from the upload request.
    /// </summary>
    /// <example>201</example>
    public int StatusCode { get; set; }

    /// <summary>
    /// The ID of the job created when the publishing process runs asynchronously.
    /// This property is populated only if <see cref="Input.AsJob"/> is set to true.
    /// </summary>
    public string JobId { get; set; }
}