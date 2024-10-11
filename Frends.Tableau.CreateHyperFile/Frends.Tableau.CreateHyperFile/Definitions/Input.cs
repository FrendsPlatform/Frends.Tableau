namespace Frends.Tableau.CreateHyperFile.Definitions;

/// <summary>
/// Represents the input data required for the operation.
/// </summary>
public class Input
{
    /// <summary>
    /// Gets or sets the file path where the data is stored.
    /// </summary>
    /// <example>c:\\temp\file.hyper</example>
    public string Path { get; set; }

    /// <summary>
    /// Gets or sets the JSON string containing the input data.
    /// </summary>
    /// <example>
    /// [
    ///     {
    ///         "Name": "John Doe",
    ///         "Age": 30,
    ///         "Salary": 50000.50,
    ///         "IsActive": true,
    ///         "JoiningDate": "2023-10-11",
    ///         "BinaryData": "VGhpcyBpcyBzb21lIGJpbmFyeSBkYXRh"
    ///     },
    ///     {
    ///         "Name": "Jane Smith",
    ///         "Age": 25,
    ///         "Salary": 45000.75,
    ///         "IsActive": false,
    ///         "JoiningDate": "2024-01-01",
    ///         "BinaryData": "U29tZSBvdGhlciBiaW5hcnkgZGF0YQ=="
    ///     }
    /// ]
    /// </example>
    public string Json { get; set; }
}