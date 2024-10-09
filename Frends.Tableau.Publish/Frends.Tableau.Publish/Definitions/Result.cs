using System.Collections.Generic;

namespace Frends.Tableau.Publish.Definitions;

/// <summary>
/// Result class.
/// </summary>
public class Result
{
    internal Result(List<FileUploadResult> fileUploadResults)
    {
        Data = fileUploadResults;
    }

    /// <summary>
    /// List of file upload results.
    /// </summary>
    /// <example>
    /// [
    ///   { "File": "c:\\temp\\file1.tflx", "Success": true, "Message": "Message", "StatusCode": 202, "JobId": null },
    ///   { "File": "c:\\temp\\file2.tflx", "Success": true, "Message": "Message", "StatusCode": 202, "JobId": "321" },
    ///   { "File": "c:\\temp\\file3.tflx", "Success": true, "Message": "Message", "StatusCode": 202, "JobId": "123" }
    /// ]
    /// </example>
    public List<FileUploadResult> Data { get; private set; }
}