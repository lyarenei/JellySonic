using System.Xml.Serialization;

namespace JellySonic.Subsonic;

/// <summary>
/// Base Subsonic response.
/// </summary>
[XmlRoot(ElementName = "subsonic-response", Namespace = "http://subsonic.org/restapi")]
public class BaseResponse
{
    /// <summary>
    /// Reported version of the Subsonic server.
    /// </summary>
    private const string ServerVer = "1.12.0";

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseResponse"/> class.
    /// </summary>
    public BaseResponse()
    {
        this.Status = "ok";
        this.Version = ServerVer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseResponse"/> class.
    /// </summary>
    /// <param name="status">A response status.</param>
    public BaseResponse(string status = "ok")
    {
        this.Status = status;
        this.Version = ServerVer;
    }

    /// <summary>
    /// Gets or sets the Subsonic REST API version.
    /// </summary>
    [XmlAttribute("version")]
    public string Version { get; set; }

    /// <summary>
    /// Gets or sets a response status.
    /// </summary>
    [XmlAttribute("status")]
    public string Status { get; set; }
}
