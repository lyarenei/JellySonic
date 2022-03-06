using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        SongCount = 0;
        Duration = 0;
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
            SongCount = album.Tracks.Count();
        }
        catch
        {
            Artist = string.Empty;
            SongCount = 0;
        }

        Id = item.Id.ToString();
        Name = item.Name;
        ArtistId = item.DisplayParentId.ToString();
        CoverArt = item.Id.ToString();
        Duration = Utils.Utils.TicksToSeconds(item.RunTimeTicks);
        Created = item.DateCreated.ToString(Utils.Utils.IsoDateFormat, DateTimeFormatInfo.InvariantInfo);
        Genre = item.Genres.Length > 0 ? item.Genres[0] : null;
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
    /// Gets or sets cover art ID of the album.
    /// </summary>
    [XmlAttribute("coverArt")]
    public string? CoverArt { get; set; }

    /// <summary>
    /// Gets or sets number of songs in the album.
    /// </summary>
    [XmlAttribute("songCount")]
    public int SongCount { get; set; }

    /// <summary>
    /// Gets or sets album duration in seconds.
    /// </summary>
    [XmlAttribute("duration")]
    public long Duration { get; set; }

    /// <summary>
    /// Gets or sets album play count.
    /// </summary>
    [XmlIgnore]
    public int? PlayCount { get; set; }

    /// <summary>
    /// Gets or sets album play count for serialization.
    /// </summary>
    [XmlAttribute("playCount")]
    [JsonIgnore]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int PlayCountSerialized
    {
        get { return PlayCount ?? -1; }
        set { PlayCount = value; }
    }

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
    /// Gets or sets release year of the album.
    /// </summary>
    [XmlIgnore]
    public int? Year { get; set; }

    /// <summary>
    /// Gets or sets album release year for serialization.
    /// </summary>
    [XmlAttribute("year")]
    [JsonIgnore]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int YearSerialized
    {
        get { return Year ?? -1; }
        set { Year = value; }
    }

    /// <summary>
    /// Gets or sets genre of the album.
    /// </summary>
    [XmlAttribute("genre")]
    public string? Genre { get; set; }

    /// <summary>
    /// Determines if album play count should be serialized.
    /// </summary>
    /// <returns>True if album count has a value.</returns>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializePlayCountSerialized() => PlayCount != null;

    /// <summary>
    /// Determines if album release year should be serialized.
    /// </summary>
    /// <returns>True if album year has a value.</returns>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeYearSerialized() => Year != null;
}

/// <summary>
/// Subsonic AlbumWithSongsID3 data type.
/// </summary>
public class AlbumWithSongsId3 : AlbumId3, IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumWithSongsId3"/> class.
    /// </summary>
    public AlbumWithSongsId3()
    {
        Songs = new Collection<Child>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumWithSongsId3"/> class.
    /// </summary>
    /// <param name="item">Music Album.</param>
    public AlbumWithSongsId3(MusicAlbum item) : base(item)
    {
        var songList = item.Tracks.Select(track => new Child(track)).OrderBy(child => child.Track);
        Songs = new Collection<Child>(songList.ToList());
    }

    /// <summary>
    /// Gets songs of album.
    /// </summary>
    [XmlElement("song")]
    [JsonPropertyName("song")]
    public Collection<Child> Songs { get; }
}
