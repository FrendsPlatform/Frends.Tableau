namespace Frends.Tableau.Publish.Definitions;

/// <summary>
/// Types of authentication methods.
/// </summary>
public enum AuthenticationTypes
{
    /// <summary>
    /// Basic authentication using username and password.
    /// </summary>
    Basic,

    /// <summary>
    /// Personal Access Token (PAT) authentication.
    /// </summary>
    PAT,

    /// <summary>
    /// Credentials token.
    /// </summary>
    CredentialsToken,
}

/// <summary>
/// Types of Tableau resources.
/// </summary>
public enum ResourceTypes
{
    /// <summary>
    /// Tableau workbooks.
    /// </summary>
    Workbooks,

    /// <summary>
    /// Tableau flows.
    /// </summary>
    Flows,

    /// <summary>
    /// Tableau data sources.
    /// </summary>
    DataSources,
}