using System.Xml.Serialization;
using JellySonic.Types;

namespace JellySonic.Models;

/// <summary>
/// A Subsonic licence response.
/// </summary>
public class License : IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="License"/> class.
    /// </summary>
    public License()
    {
        Valid = "true";
    }

    /// <summary>
    /// Gets or sets if a licence is valid.
    /// </summary>
    [XmlAttribute("valid")]
    public string Valid { get; set; }

    /// <summary>
    /// Gets or sets a licence owner's e-mail.
    /// </summary>
    [XmlAttribute("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets licence expiration date.
    /// </summary>
    [XmlAttribute("licenseExpires")]
    public string? LicenseExpires { get; set; }

    /// <summary>
    /// Gets or sets trial expiration date.
    /// </summary>
    [XmlAttribute("trialExpires")]
    public string? TrialExpires { get; set; }
}
