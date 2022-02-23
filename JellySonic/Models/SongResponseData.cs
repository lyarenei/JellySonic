using System;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using MediaBrowser.Controller.Entities.Audio;
using static System.IO.Path;
using static JellySonic.Utils.Utils;

namespace JellySonic.Models;

/// <summary>
/// A Subsonic song response data.
/// </summary>
public class SongResponseData : ResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SongResponseData"/> class.
    /// </summary>
    public SongResponseData()
    {
        Id = string.Empty;
        ParentId = string.Empty;
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
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SongResponseData"/> class.
    /// </summary>
    /// <param name="audio">Audio item.</param>
    public SongResponseData(Audio audio)
    {
        Id = audio.Id.ToString();
        ParentId = string.Empty;
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
    public string ParentId { get; set; }

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
}
