using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using MediaBrowser.Controller.Entities.Audio;
using static System.IO.Path;
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
public class Song
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Song"/> class.
    /// </summary>
    public Song()
    {
        Id = string.Empty;
        Parent = string.Empty;
        Title = string.Empty;
        AlbumName = string.Empty;
        ArtistName = string.Empty;
        IsDir = "false";
        CoverArt = string.Empty;
        Created = DateTime.Now;
        Duration = "0";
        BitRate = "0";
        FileSize = "0";
        FilenameSuffix = string.Empty;
        ContentType = string.Empty;
        IsVideo = "false";
        Path = string.Empty;
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
    public Song(Audio audio)
    {
        Id = audio.Id.ToString();
        Parent = string.Empty;
        Title = audio.Name;
        AlbumName = audio.Album;
        ArtistName = audio.Artists.FirstOrDefault(string.Empty);
        IsDir = "false";
        CoverArt = string.Empty;
        Created = audio.DateCreated;
        Duration = TicksToSeconds(audio.RunTimeTicks).ToString(NumberFormatInfo.InvariantInfo);
        BitRate = (audio.TotalBitrate / 1000).ToString() ?? "0";
        FileSize = audio.Size?.ToString(NumberFormatInfo.InvariantInfo) ?? "0";
        FilenameSuffix = GetExtension(audio.Path)[1..];
        ContentType = string.Empty;
        IsVideo = "false";
        Path = audio.Path;
        AlbumId = audio.AlbumEntity.Id.ToString();
        ArtistId = string.Empty;
        Type = "music";
        Year = audio.ProductionYear.ToString() ?? "1970";
        Genre = audio.Genres.FirstOrDefault(string.Empty);
        TrackOrder = audio.IndexNumber?.ToString(NumberFormatInfo.InvariantInfo) ?? "0";
    }

    /// <summary>
    /// Gets or sets ID of the song.
    /// </summary>
    [XmlAttribute("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets something.
    /// No idea what is this.
    /// TODO - find out what is this.
    /// </summary>
    [XmlAttribute("parent")]
    public string Parent { get; set; }

    /// <summary>
    /// Gets or sets name of the song.
    /// </summary>
    [XmlAttribute("title")]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets album name of the song.
    /// </summary>
    [XmlAttribute("album")]
    public string AlbumName { get; set; }

    /// <summary>
    /// Gets or sets artist name of the song.
    /// </summary>
    [XmlAttribute("artist")]
    public string ArtistName { get; set; }

    /// <summary>
    /// Gets or sets if song is a directory.
    /// </summary>
    [XmlAttribute("isDir")]
    public string IsDir { get; set; }

    /// <summary>
    /// Gets or sets cover art of the song.
    /// </summary>
    [XmlAttribute("coverArt")]
    public string CoverArt { get; set; }

    /// <summary>
    /// Gets or sets date of song creation.
    /// </summary>
    [XmlAttribute("created")]
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets or sets duration of the song.
    /// </summary>
    [XmlAttribute("duration")]
    public string Duration { get; set; }

    /// <summary>
    /// Gets or sets bitrate of the song.
    /// </summary>
    [XmlAttribute("bitRate")]
    public string BitRate { get; set; }

    /// <summary>
    /// Gets or sets filesize of the song.
    /// </summary>
    [XmlAttribute("size")]
    public string FileSize { get; set; }

    /// <summary>
    /// Gets or sets filename suffix of the song.
    /// </summary>
    [XmlAttribute("suffix")]
    public string FilenameSuffix { get; set; }

    /// <summary>
    /// Gets or sets content type of the song.
    /// TODO - probably audio/something.
    /// </summary>
    [XmlAttribute("contentType")]
    public string ContentType { get; set; }

    /// <summary>
    /// Gets or sets if song is a video.
    /// </summary>
    [XmlAttribute("isVideo")]
    public string IsVideo { get; set; }

    /// <summary>
    /// Gets or sets path to the song filename.
    /// </summary>
    [XmlAttribute("path")]
    public string Path { get; set; }

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
