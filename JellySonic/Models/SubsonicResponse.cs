using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using JellySonic.Services;
using MediaBrowser.Model.IO;
using Microsoft.AspNetCore.Mvc;

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
    /// <param name="status">A response status.</param>
    public SubsonicResponse(string status = "ok")
    {
        Status = status;
        Version = ServerVer;
    }

    /// <summary>
    /// Gets or sets response data.
    /// </summary>
    [XmlIgnore]
    public ResponseData? ResponseData { get; set; }

    /// <summary>
    /// Gets or sets response data for serialization.
    /// </summary>
    [XmlElement("artists", typeof(Artists))]
    [XmlElement("error", typeof(SubsonicError))]
    [XmlElement("license", typeof(License))]
    public object? ResponseDataSerialization
    {
        get { return ResponseData; }
        set { ResponseData = (ResponseData)value!; }
    }

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

    /// <summary>
    /// Converts object to memory stream.
    /// </summary>
    /// <returns>Memory stream with object data.</returns>
    public MemoryStream ToMemoryStream()
    {
        // Taken from https://github.com/jellyfin/jellyfin-plugin-opds/blob/b78a8bcc979581fe92835235a2c0d59516b5df15/Jellyfin.Plugin.Opds/OpdsApi.cs#L294
        var memoryStream = new MemoryStream();
        var serializer = XmlHelper.Create(typeof(SubsonicResponse), Namespace);
        using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, IODefaults.StreamWriterBufferSize, true))
        using (var textWriter = new XmlTextWriter(writer))
        {
            textWriter.Formatting = Formatting.Indented;
            var emptyNamespaces = new XmlSerializerNamespaces();
            emptyNamespaces.Add(string.Empty, string.Empty);
            serializer.Serialize(textWriter, this, emptyNamespaces);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }
}
