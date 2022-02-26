using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using JellySonic.Types;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using static System.IO.Path;
using static JellySonic.Utils.Utils;

namespace JellySonic.Models;

/// <summary>
/// Subsonic Child data type.
/// </summary>
public class Child : IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Child"/> class.
    /// </summary>
    public Child()
    {
        Id = string.Empty;
        IsDir = string.Empty;
        Title = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Child"/> class.
    /// </summary>
    /// <param name="item">General item.</param>
    public Child(BaseItem item)
    {
        Id = item.Id.ToString();
        Parent = item.ParentId.ToString();
        Title = item.Name;
        IsDir = item.IsFolder.ToString();
        Album = item.Album;

        try
        {
            Artist = ((Audio)item).AlbumArtists.FirstOrDefault(string.Empty);
        }
        catch
        {
            Artist = string.Empty;
        }

        Track = item.IndexNumber.ToString();
        Year = item.ProductionYear.ToString();
        Genre = item.Genres.FirstOrDefault(string.Empty);
        CoverArt = item.Id.ToString();
        Size = item.Size?.ToString(NumberFormatInfo.InvariantInfo);

        var ext = GetExtension(item.Path);
        Suffix = string.IsNullOrEmpty(ext) ? null : ext[1..];

        Duration = TicksToSeconds(item.RunTimeTicks).ToString(NumberFormatInfo.InvariantInfo);
        BitRate = (item.TotalBitrate / 1000).ToString();
        Path = item.Path;
    }

    /// <summary>
    /// Gets or sets ID of the child item.
    /// </summary>
    [XmlAttribute("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets ID of the parent of this child item.
    /// </summary>
    [XmlAttribute("parent")]
    public string? Parent { get; set; }

    /// <summary>
    /// Gets or sets if this child item is a directory.
    /// </summary>
    [XmlAttribute("isDir")]
    public string IsDir { get; set; }

    /// <summary>
    /// Gets or sets name of the child item.
    /// </summary>
    [XmlAttribute("title")]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets album name of the child item.
    /// </summary>
    [XmlAttribute("album")]
    public string? Album { get; set; }

    /// <summary>
    /// Gets or sets artist name of the child item.
    /// </summary>
    [XmlAttribute("artist")]
    public string? Artist { get; set; }

    /// <summary>
    /// Gets or sets track order of child item.
    /// </summary>
    [XmlAttribute("track")]
    public string? Track { get; set; }

    /// <summary>
    /// Gets or sets release year of the child item.
    /// </summary>
    [XmlAttribute("year")]
    public string? Year { get; set; }

    /// <summary>
    /// Gets or sets genre of the child item.
    /// </summary>
    [XmlAttribute("genre")]
    public string? Genre { get; set; }

    /// <summary>
    /// Gets or sets cover art of the child item.
    /// </summary>
    [XmlAttribute("coverArt")]
    public string? CoverArt { get; set; }

    /// <summary>
    /// Gets or sets filesize of the child item.
    /// </summary>
    [XmlAttribute("size")]
    public string? Size { get; set; }

    /// <summary>
    /// Gets or sets content type of the child item.
    /// For example audio/mp3.
    /// </summary>
    [XmlAttribute("contentType")]
    public string? ContentType { get; set; }

    /// <summary>
    /// Gets or sets filename suffix of the child item.
    /// </summary>
    [XmlAttribute("suffix")]
    public string? Suffix { get; set; }

    /// <summary>
    /// Gets or sets transcoded content type of the child item.
    /// </summary>
    [XmlAttribute("transcodedContentType")]
    public string? TranscodedContentType { get; set; }

    /// <summary>
    /// Gets or sets transcoded filename suffix of the child item.
    /// </summary>
    [XmlAttribute("transcodedSuffix")]
    public string? TranscodedSuffix { get; set; }

    /// <summary>
    /// Gets or sets duration of the child item.
    /// </summary>
    [XmlAttribute("duration")]
    public string? Duration { get; set; }

    /// <summary>
    /// Gets or sets bitrate of the child item.
    /// </summary>
    [XmlAttribute("bitRate")]
    public string? BitRate { get; set; }

    /// <summary>
    /// Gets or sets path to the child item filename.
    /// </summary>
    [XmlAttribute("path")]
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets if child item is a video.
    /// </summary>
    [XmlAttribute("isVideo")]
    public string? IsVideo { get; set; }

    /// <summary>
    /// Gets or sets child item user rating.
    /// </summary>
    [XmlAttribute("userRating")]
    public string? UserRating { get; set; }

    /// <summary>
    /// Gets or sets child item average rating.
    /// </summary>
    [XmlAttribute("averageRating")]
    public string? AverageRating { get; set; }

    /// <summary>
    /// Gets or sets child item play count.
    /// </summary>
    [XmlAttribute("playCount")]
    public string? PlayCount { get; set; }

    /// <summary>
    /// Gets or sets child item disc number.
    /// </summary>
    [XmlAttribute("discNumber")]
    public string? DiscNumber { get; set; }

    /// <summary>
    /// Gets or sets if date of starred child item.
    /// </summary>
    [XmlAttribute("created")]
    public string? Created { get; set; }

    /// <summary>
    /// Gets or sets if date of starred child item.
    /// </summary>
    [XmlAttribute("starred")]
    public string? Starred { get; set; }

    /// <summary>
    /// Gets or sets ID of the child item's album.
    /// </summary>
    [XmlAttribute("albumId")]
    public string? AlbumId { get; set; }

    /// <summary>
    /// Gets or sets ID of the child item's artist.
    /// </summary>
    [XmlAttribute("artistId")]
    public string? ArtistId { get; set; }

    /// <summary>
    /// Gets or sets media type of the child item.
    /// </summary>
    [XmlAttribute("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets bookmark position in the child item.
    /// </summary>
    [XmlAttribute("bookmarkPosition")]
    public string? BookmarkPosition { get; set; }

    /// <summary>
    /// Gets or sets original width of the child item.
    /// </summary>
    [XmlAttribute("originalWidth")]
    public string? OriginalWidth { get; set; }

    /// <summary>
    /// Gets or sets original height of the child item.
    /// </summary>
    [XmlAttribute("originalHeight")]
    public string? OriginalHeight { get; set; }
}
