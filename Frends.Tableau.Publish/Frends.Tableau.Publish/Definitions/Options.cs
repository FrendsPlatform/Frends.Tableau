namespace Frends.Tableau.Publish.Definitions;
using System.ComponentModel;

/// <summary>
/// Contains the optional parameters for the publish operation.
/// </summary>
public class Options
{
    /// <summary>
    /// Indicates whether to new project should be created if not found
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool CreateProject { get; set; }
}