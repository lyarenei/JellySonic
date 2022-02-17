using System.Xml.Serialization;

namespace JellySonic.Models;

/// <summary>
/// Base Subsonic response.
/// </summary>
[XmlRoot(ElementName = "subsonic-response")]
public class SubsonicResponse
{
    /// <summary>
    /// Reported version of the Subsonic server.
    /// </summary>
    private const string ServerVer = "1.12.0";

    /// <summary>
    /// Gets namespace.
    /// </summary>
    public const string Namespace = "http://subsonic.org/restapi";

    /// <summary>
    /// Initializes a new instance of the <see cref="SubsonicResponse"/> class.
    /// </summary>
    public SubsonicResponse()
    {
        ResponseData = null;
        Status = "ok";
        Version = ServerVer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubsonicResponse"/> class.
    /// </summary>
    /// <param name="responseData">Response data.</param>
    /// <param name="status">A response status.</param>
    public SubsonicResponse(ResponseData responseData, string status = "ok")
    {
        ResponseData = responseData;
        Status = status;
        Version = ServerVer;
    }

    /// <summary>
    /// Gets or sets response data.
    /// </summary>
    [XmlElement]
    public ResponseData? ResponseData { get; set; }

    /// <summary>
    /// Gets or sets a response status.
    /// </summary>
    [XmlAttribute("status")]
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the Subsonic REST API version.
    /// </summary>
    [XmlAttribute("version")]
    public string Version { get; set; }
}
