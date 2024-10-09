namespace Frends.Tableau.Publish.Definitions;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;

/// <summary>
/// Connection details for Tableau authentication and interaction.
/// </summary>
public class Connection
{
    /// <summary>
    /// Type of authentication.
    /// </summary>
    /// <example>AuthenticationTypes.Basic</example>
    [DefaultValue(AuthenticationTypes.Basic)]
    public AuthenticationTypes AuthenticationType { get; set; }

    /// <summary>
    /// Base URL for the Tableau API.
    /// </summary>
    /// <example>https://prod-uk-a.online.tableau.com/api/3.4/</example>
    public string BaseUrl { get; set; }

    /// <summary>
    /// Name of the Tableau site.
    /// </summary>
    /// <example>user-1234fca6b1</example>
    public string SiteName { get; set; }

    /// <summary>
    /// Username for authentication.
    /// </summary>
    /// <example>User</example>
    [UIHint(nameof(AuthenticationType), "", AuthenticationTypes.Basic)]
    public string UserName { get; set; }

    /// <summary>
    /// Password for authentication.
    /// </summary>
    /// <example>Password</example>
    [UIHint(nameof(AuthenticationType), "", AuthenticationTypes.Basic)]
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    public string Password { get; set; }

    /// <summary>
    /// Name of the Personal Access Token (PAT).
    /// </summary>
    /// <example>PAT</example>
    [UIHint(nameof(AuthenticationType), "", AuthenticationTypes.PAT)]
    public string PatName { get; set; }

    /// <summary>
    /// Secret for the Personal Access Token (PAT).
    /// </summary>
    /// <example>PATsecret</example>
    [UIHint(nameof(AuthenticationType), "", AuthenticationTypes.PAT)]
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    public string PatSecret { get; set; }

    /// <summary>
    /// Token used for authenticating API requests.
    /// </summary>
    /// <example>abc123xyz</example>
    [UIHint(nameof(AuthenticationType), "", AuthenticationTypes.CredentialsToken)]
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    public string CredentialsToken { get; set; }

    /// <summary>
    /// Identifier for the Tableau site.
    /// </summary>
    /// <example>site-id-5678</example>
    [UIHint(nameof(AuthenticationType), "", AuthenticationTypes.CredentialsToken)]
    public string SiteId { get; set; }
}