namespace Frends.Tableau.Publish.Definitions;

/// <summary>
/// Result class.
/// </summary>
public class Result
{
    internal Result(FileUploadResult[] fileUploadResults)
    {
        FileUploadResults = fileUploadResults;
    }

    /// <summary>
    /// List of file upload results.
    /// </summary>
    /// <example>
    /// { {"File", true, "Message", 202, null}, {"File2", true, "Message", 202, null} }
    /// { {"File", true, "Message", 202, null}, {"File2", true, "Message", 202, "321"} }
    /// { {"File", true, "Message", 202, "123"}, {"File2", true, "Message", 202, "321"} }
    /// </example>
    public FileUploadResult[] FileUploadResults { get; private set; }
}