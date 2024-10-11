namespace Frends.Tableau.CreateHyperFile.Definitions;

/// <summary>
/// Represents the result of an operation..
/// </summary>
public class Result
{
    internal Result(bool success, int recordsAffected, string message)
    {
        Success = success;
        RecordsAffected = recordsAffected;
        Message = message;
    }

    /// <summary>
    /// Indicates whether the Task was completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// The number of records affected by the operation.
    /// </summary>
    /// <example>2</example>
    public int RecordsAffected { get; set; }

    /// <summary>
    /// A message describing the result of the operation.
    /// </summary>
    /// <example></example>
    public string Message { get; set; }
}