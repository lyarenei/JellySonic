using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using JellySonic.Types;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;

namespace JellySonic.Models;

/// <summary>
/// A subsonic album response data.
/// </summary>
public class AlbumId3
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumId3"/> class.
    /// </summary>
    public AlbumId3()
    {
        Id = string.Empty;
        Name = string.Empty;
        SongCount = string.Empty;
        Duration = string.Empty;
        Created = DateTime.Now.ToString(Utils.Utils.IsoDateFormat, DateTimeFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumId3"/> class.
    /// </summary>
    /// <param name="item">Album item.</param>
    public AlbumId3(BaseItem item)
    {
        try
        {
            var album = (MusicAlbum)item;
            Artist = album.AlbumArtist;
            SongCount = album.Tracks.Count().ToString(NumberFormatInfo.InvariantInfo);
        }
        catch
        {
            Artist = string.Empty;
            SongCount = string.Empty;
        }

        Id = item.Id.ToString();
        Name = item.Name;
        ArtistId = item.DisplayParentId.ToString();
        CoverArt = item.Id.ToString();
        Duration = Utils.Utils.TicksToSeconds(item.RunTimeTicks).ToString(NumberFormatInfo.InvariantInfo);
        Created = item.DateCreated.ToString(Utils.Utils.IsoDateFormat, DateTimeFormatInfo.InvariantInfo);
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
    /// Gets or sets artist name of the album.
    /// </summary>
    [XmlAttribute("artist")]
    public string? Artist { get; set; }

    /// <summary>
    /// Gets or sets ID of the artist of the album.
    /// </summary>
    [XmlAttribute("artistId")]
    public string? ArtistId { get; set; }

    /// <summary>
    /// Gets or sets cover art of the album.
    /// </summary>
    [XmlAttribute("coverArt")]
    public string? CoverArt { get; set; }

    /// <summary>
    /// Gets or sets number of songs in the album.
    /// </summary>
    [XmlAttribute("songCount")]
    public string SongCount { get; set; }

    /// <summary>
    /// Gets or sets album duration in seconds.
    /// </summary>
    [XmlAttribute("duration")]
    public string Duration { get; set; }

    /// <summary>
    /// Gets or sets album play count.
    /// </summary>
    [XmlAttribute("playCount")]
    public string? PlayCount { get; set; }

    /// <summary>
    /// Gets or sets date of album creation.
    /// </summary>
    [XmlAttribute("created")]
    public string Created { get; set; }

    /// <summary>
    /// Gets or sets album starred date.
    /// </summary>
    [XmlAttribute("starred")]
    public string? Starred { get; set; }

    /// <summary>
    /// Gets or sets release year of the song.
    /// </summary>
    [XmlAttribute("year")]
    public string? Year { get; set; }

    /// <summary>
    /// Gets or sets genre of the song.
    /// </summary>
    [XmlAttribute("genre")]
    public string? Genre { get; set; }
}

/// <summary>
/// Subsonic AlbumWithSongsID3 data type.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class AlbumWithSongsId3 : AlbumId3, IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumWithSongsId3"/> class.
    /// </summary>
    public AlbumWithSongsId3()
    {
        Songs = new List<Child>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumWithSongsId3"/> class.
    /// </summary>
    /// <param name="item">Music Album.</param>
    public AlbumWithSongsId3(MusicAlbum item) : base(item)
    {
        Songs = item.Tracks.Select(track => new Child(track)).ToList();
    }

    /// <summary>
    /// Gets or sets songs of album.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<Child> Songs { get; set; }

    /// <summary>
    /// Gets or sets songs of album for serialization.
    /// </summary>
    [XmlElement("song")]
    [JsonPropertyName("song")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<Child> SongsSerialize
    {
        get { return Songs.ToList(); }
        set { Songs = value; }
    }
}
