using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using JellySonic.Json;
using JellySonic.Services;
using JellySonic.Types;
using MediaBrowser.Model.IO;

namespace JellySonic.Models;

/// <summary>
/// Base Subsonic response.
/// </summary>
[XmlRoot("subsonic-response")]
public class SubsonicResponse
{
    /// <summary>
    /// Reported version of the Subsonic server.
    /// </summary>
    private const string ServerVer = "1.13.0";

    /// <summary>
    /// Gets namespace.
    /// </summary>
    public const string Namespace = "http://subsonic.org/restapi";

    /// <summary>
    /// Initializes a new instance of the <see cref="SubsonicResponse"/> class.
    /// </summary>
    public SubsonicResponse()
    {
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
    /// Memory stream types.
    /// </summary>
    public enum MemoryStreamFormat
    {
        /// <summary>
        /// Use XML data format.
        /// </summary>
        Xml,

        /// <summary>
        /// Use JSON data format.
        /// </summary>
        Json
    }

    /// <summary>
    /// Gets or sets response data.
    /// </summary>
    [XmlIgnore]
    public IResponseData? ResponseData { get; set; }

    /// <summary>
    /// Gets or sets response data for serialization.
    /// </summary>
    [XmlElement("album", typeof(AlbumWithSongsId3))]
    [XmlElement("artist", typeof(ArtistWithAlbumsId3))]
    [XmlElement("artists", typeof(ArtistsId3))]
    [XmlElement("license", typeof(License))]
    [XmlElement("song", typeof(Child))]
    [XmlElement("musicFolders", typeof(MusicFolders))]
    [XmlElement("directory", typeof(SubsonicDirectory))]
    [XmlElement("error", typeof(SubsonicError))]
    [XmlElement("genres", typeof(Genres))]
    [XmlElement("indexes", typeof(Indexes))]
    [XmlElement("albumList", typeof(AlbumList))]
    [XmlElement("albumList2", typeof(AlbumList2))]
    [XmlElement("searchresult2", typeof(SearchResult2))]
    [XmlElement("searchresult3", typeof(SearchResult3))]
    [XmlElement("user", typeof(SubsonicUser))]
    [XmlElement("artistInfo", typeof(ArtistInfo))]
    [XmlElement("artistInfo2", typeof(ArtistInfo2))]
    public object? ResponseDataSerialization
    {
        get { return ResponseData; }
        set { ResponseData = (IResponseData)value!; }
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
    /// <param name="streamFormat"><see cref="MemoryStreamFormat"/>.</param>
    /// <returns>Memory stream with object data.</returns>
    public MemoryStream ToMemoryStream(MemoryStreamFormat streamFormat = MemoryStreamFormat.Xml)
    {
        return streamFormat switch
        {
            MemoryStreamFormat.Json => this.ToJsonMemoryStream(),
            _ => this.ToXmlMemoryStream()
        };
    }

    /// <summary>
    /// Convert format string to enum.
    /// Defaults to XML.
    /// </summary>
    /// <param name="format">Format as string.</param>
    /// <returns><see cref="MemoryStreamFormat"/>.</returns>
    public static MemoryStreamFormat FormatFromString(string format)
    {
        return format switch
        {
            "json" => MemoryStreamFormat.Json,
            _ => MemoryStreamFormat.Xml
        };
    }

    private MemoryStream ToXmlMemoryStream()
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

    private MemoryStream ToJsonMemoryStream()
    {
        var memoryStream = new MemoryStream();
        var serializerOpts = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        serializerOpts.Converters.Add(new SubsonicResponseJsonConverter());

        JsonSerializer.Serialize(memoryStream, this, serializerOpts);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }
}
