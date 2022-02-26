using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using JellySonic.Types;
using MediaBrowser.Controller.Entities;

namespace JellySonic.Models;

/// <summary>
/// Subsonic ArtistID3 data type.
/// </summary>
public class ArtistId3
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistId3"/> class.
    /// </summary>
    public ArtistId3()
    {
        Id = string.Empty;
        Name = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistId3"/> class.
    /// </summary>
    /// <param name="item">Artist data.</param>
    public ArtistId3(BaseItem item)
    {
        Id = item.Id.ToString();
        Name = item.Name;
        CoverArt = item.Id.ToString();
    }

    /// <summary>
    /// Gets or sets artist ID.
    /// </summary>
    [XmlAttribute("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets artist name.
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets artist cover art id.
    /// </summary>
    [XmlAttribute("coverArt")]
    public string? CoverArt { get; set; }

    /// <summary>
    /// Gets or sets URL to image of the artist.
    /// </summary>
    [XmlAttribute("artistImageUrl")]
    public string? ArtistImageUrl { get; set; }

    /// <summary>
    /// Gets or sets artist album count.
    /// </summary>
    [XmlAttribute("albumCount")]
    public string? AlbumCount { get; set; }

    /// <summary>
    /// Gets or sets if date of starred artist.
    /// </summary>
    [XmlAttribute("starred")]
    public string? Starred { get; set; }
}

/// <summary>
/// Subsonic ArtistWithAlbumsID3 data type.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class ArtistWithAlbumsId3 : ArtistId3, IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistWithAlbumsId3"/> class.
    /// </summary>
    public ArtistWithAlbumsId3()
    {
        Albums = new List<AlbumId3>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistWithAlbumsId3"/> class.
    /// </summary>
    /// <param name="artist">Artist data.</param>
    /// <param name="albums">Album collection for artist.</param>
    public ArtistWithAlbumsId3(BaseItem artist, IEnumerable<BaseItem> albums) : base(artist)
    {
        Albums = albums.Select(album => new AlbumId3(album)).ToList();
    }

    /// <summary>
    /// Gets or sets list of albums.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<AlbumId3> Albums { get; set; }

    /// <summary>
    /// Gets or sets list of albums for serialization.
    /// </summary>
    [XmlElement("album")]
    [JsonPropertyName("album")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<AlbumId3> AlbumsSerialize
    {
        get { return Albums.ToList(); }
        set { Albums = value; }
    }
}
