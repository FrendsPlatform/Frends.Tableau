namespace Frends.Tableau.CreateHyperFile.Definitions;

using System.ComponentModel;

/// <summary>
/// Contains the optional parameters for the publish operation.
/// </summary>
public class Options
{

    /// <summary>
    /// Determines whether to throw exceptions or return a Result object.
    /// If set to false, a Result object is returned with Success set to false, the number of records affected before the exception, and the exception message.
    /// If set to true, exceptions are thrown.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowException { get; set; }

    /// <summary>
    /// Determines whether to create the directory if it does not exist.
    /// If set to true, the directory will be created. If set to false, an exception will be thrown if the directory does not exist.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool CreateDirectory { get; set; }

    /// <summary>
    /// Specifies the maximum number of log files.
    /// </summary>
    /// <example>2</example>
    [DefaultValue("2")]
    public string LogFileMaxCount { get; set; }

    /// <summary>
    /// Specifies the maximum size of each log file.
    /// </summary>
    /// <example>100M</example>
    [DefaultValue("100M")]
    public string LogFileSizeLimit { get; set; }
}