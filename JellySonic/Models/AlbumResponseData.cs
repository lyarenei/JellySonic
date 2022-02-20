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
/// A subsonic album response data.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class AlbumResponseData : ResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumResponseData"/> class.
    /// </summary>
    public AlbumResponseData()
    {
        Artist = string.Empty;
        ArtistId = string.Empty;
        CoverArt = string.Empty;
        Created = DateTime.Now;
        Duration = "0";
        Id = string.Empty;
        Name = string.Empty;
        Songs = new List<Song>();
        SongCount = "0";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumResponseData"/> class.
    /// </summary>
    /// <param name="album">Album item.</param>
    public AlbumResponseData(MusicAlbum album)
    {
        Artist = album.AlbumArtist;
        ArtistId = album.DisplayParentId.ToString();
        CoverArt = string.Empty;
        Created = album.DateCreated;
        Duration = TicksToSeconds(album.RunTimeTicks).ToString(NumberFormatInfo.InvariantInfo);
        Id = album.Id.ToString();
        Name = album.Name;
        Songs = album.Tracks.Select(track => new Song(track)).ToList();
        SongCount = album.Tracks.Count().ToString(NumberFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Gets or sets artist name of the album.
    /// </summary>
    [XmlAttribute("artist")]
    public string Artist { get; set; }

    /// <summary>
    /// Gets or sets iD of the artist of the album.
    /// </summary>
    [XmlAttribute("artistId")]
    public string ArtistId { get; set; }

    /// <summary>
    /// Gets or sets cover art of the album.
    /// </summary>
    [XmlAttribute("coverArt")]
    public string CoverArt { get; set; }

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
    /// Gets or sets songs of album.
    /// </summary>
    [XmlIgnore]
    public IEnumerable<Song> Songs { get; set; }

    /// <summary>
    /// Gets or sets songs of album for serialization.
    /// </summary>
    [XmlElement("song")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<Song> SongsSurrogate
    {
        get { return Songs.ToList(); }
        set { Songs = value; }
    }

    /// <summary>
    /// Gets or sets number of songs in the album.
    /// </summary>
    [XmlAttribute("songCount")]
    public string SongCount { get; set; }
}

/// <summary>
/// foobar.
/// </summary>
[XmlRoot(ElementName="song")]
public class Song : SongResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Song"/> class.
    /// </summary>
    public Song()
    {
        AlbumId = string.Empty;
        ArtistId = string.Empty;
        Type = "music";
        Year = "1970";
        Genre = string.Empty;
        TrackOrder = "0";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Song"/> class.
    /// </summary>
    /// <param name="audio">Audio item.</param>
    public Song(Audio audio) : base(audio)
    {
        AlbumId = audio.AlbumEntity.Id.ToString();
        ArtistId = string.Empty;
        Type = "music";
        Year = audio.ProductionYear.ToString() ?? "1970";
        Genre = audio.Genres.FirstOrDefault(string.Empty);
        TrackOrder = audio.IndexNumber?.ToString(NumberFormatInfo.InvariantInfo) ?? "0";
    }

    /// <summary>
    /// Gets or sets ID of the song's album.
    /// </summary>
    [XmlAttribute("albumId")]
    public string AlbumId { get; set; }

    /// <summary>
    /// Gets or sets ID of the song's artist.
    /// </summary>
    [XmlAttribute("artistId")]
    public string ArtistId { get; set; }

    /// <summary>
    /// Gets or sets type of the song.
    /// Should be always set to music.
    /// </summary>
    [XmlAttribute("type")]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets release year of the song.
    /// </summary>
    [XmlAttribute("year")]
    public string Year { get; set; }

    /// <summary>
    /// Gets or sets genre of the song.
    /// </summary>
    [XmlAttribute("genre")]
    public string Genre { get; set; }

    /// <summary>
    /// Gets or sets track order in the album.
    /// </summary>
    [XmlAttribute("track")]
    public string TrackOrder { get; set; }
}
