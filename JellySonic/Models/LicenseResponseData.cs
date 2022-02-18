using System.Xml.Serialization;

namespace JellySonic.Models;

/// <summary>
/// A Subsonic licence response.
/// </summary>
public class LicenseResponseData : ResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LicenseResponseData"/> class.
    /// </summary>
    public LicenseResponseData()
    {
        Email = "admin@jellyfin.server";
        ExpiresAt = "9999-12-31T23:59:59";
        IsValid = "true";
    }

    /// <summary>
    /// Gets or sets a licence owner's e-mail.
    /// </summary>
    [XmlAttribute("email")]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets licence expiration date.
    /// </summary>
    [XmlAttribute("licenseExpires")]
    public string ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets indicates if a licence is valid.
    /// </summary>
    [XmlAttribute("valid")]
    public string IsValid { get; set; }
}
