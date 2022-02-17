using System.Xml.Serialization;

namespace JellySonic.Models;

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
        Data = new object();
        Status = "ok";
        Version = ServerVer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseResponse"/> class.
    /// </summary>
    /// <param name="data">Response data.</param>
    /// <param name="status">A response status.</param>
    public BaseResponse(object data, string status = "ok")
    {
        Data = data;
        Status = status;
        Version = ServerVer;
    }

    /// <summary>
    /// Gets or sets response data.
    /// </summary>
    [XmlElement]
    [XmlArrayItem]
    public object Data { get; set; }

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
