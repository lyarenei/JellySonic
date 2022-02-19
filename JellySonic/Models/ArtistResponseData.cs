using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using MediaBrowser.Controller.Entities.Audio;
using static JellySonic.Utils.Utils;

namespace JellySonic.Models;

/// <summary>
/// A Subsonic artist response data.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class ArtistResponseData : IndexArtist
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistResponseData"/> class.
    /// </summary>
    public ArtistResponseData()
    {
        Albums = new List<Album>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistResponseData"/> class.
    /// </summary>
    /// <param name="artist">An artist item.</param>
    /// <param name="albumItems">Album items.</param>
    public ArtistResponseData(MusicArtist artist, IEnumerable<MusicAlbum> albumItems) : base(artist)
    {
        var albums = new List<Album>();
        foreach (var item in albumItems)
        {
            albums.Add(new Album(item));
        }

        Albums = albums;
    }

    /// <summary>
    /// Gets or sets list of albums.
    /// </summary>
    [XmlIgnore]
    public IEnumerable<Album> Albums { get; set; }

    /// <summary>
    /// Gets or sets list of albums used for serialization.
    /// </summary>
    [XmlElement("album")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<Album> AlbumsSurrogate
    {
        get { return Albums.ToList(); }
        set { Albums = value; }
    }
}

/// <summary>
/// An album used to list in artist response.
/// </summary>
[XmlRoot("album")]
public class Album
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Album"/> class.
    /// </summary>
    public Album()
    {
        Artist = string.Empty;
        ArtistId = string.Empty;
        CoverArt = string.Empty;
        Created = DateTime.Now;
        Duration = string.Empty;
        Id = string.Empty;
        Name = string.Empty;
        SongCount = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Album"/> class.
    /// </summary>
    /// <param name="album">Album item.</param>
    public Album(MusicAlbum album)
    {
        Artist = album.AlbumArtist;
        ArtistId = album.DisplayParentId.ToString();
        CoverArt = string.Empty;
        Created = album.DateCreated;
        Duration = TicksToSeconds(album.RunTimeTicks).ToString(NumberFormatInfo.InvariantInfo);
        Id = album.Id.ToString();
        Name = album.Name;
        SongCount = album.Tracks.Count().ToString(CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Gets or sets album ID.
    /// </summary>
    [XmlAttribute("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets album name.
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets album cover art.
    /// </summary>
    [XmlAttribute("coverArt")]
    public string CoverArt { get; set; }

    /// <summary>
    /// Gets or sets album song count.
    /// </summary>
    [XmlAttribute("songCount")]
    public string SongCount { get; set; }

    /// <summary>
    /// Gets or sets date of album creation.
    /// </summary>
    [XmlAttribute("created")]
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets or sets album duration in seconds.
    /// </summary>
    [XmlAttribute("duration")]
    public string Duration { get; set; }

    /// <summary>
    /// Gets or sets album artist.
    /// </summary>
    [XmlAttribute("artist")]
    public string Artist { get; set; }

    /// <summary>
    /// Gets or sets album artist ID.
    /// </summary>
    [XmlAttribute("artistId")]
    public string ArtistId { get; set; }
}
