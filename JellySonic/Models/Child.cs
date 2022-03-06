using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
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
        IsDir = false;
        Title = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Child"/> class.
    /// </summary>
    /// <param name="item">General item.</param>
    /// <param name="parentIsArtist">If audio item, indicates if the parent should be an artist ID. Default false => album ID.</param>
    public Child(BaseItem item, bool parentIsArtist = false)
    {
        Id = item.Id.ToString();
        try
        {
            Parent = parentIsArtist ? ((Audio)item).AlbumEntity.ParentId.ToString() : item.ParentId.ToString();
        }
        catch
        {
            Parent = item.ParentId.ToString();
        }

        Title = item.Name;
        IsDir = item.IsFolder;
        Album = item.Album;

        try
        {
            Artist = ((Audio)item).AlbumArtists.FirstOrDefault(string.Empty);
        }
        catch
        {
            Artist = string.Empty;
        }

        Track = item.IndexNumber;
        Year = item.ProductionYear;
        Genre = item.Genres.FirstOrDefault(string.Empty);
        CoverArt = item.Id.ToString();
        Size = item.Size;

        var ext = GetExtension(item.Path);
        Suffix = string.IsNullOrEmpty(ext) ? null : ext[1..];

        Duration = TicksToSeconds(item.RunTimeTicks);
        BitRate = item.TotalBitrate / 1000;
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
    /// Gets or sets a value indicating whether this child item is a directory.
    /// </summary>
    [XmlAttribute("isDir")]
    public bool IsDir { get; set; }

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
    [XmlIgnore]
    public int? Track { get; set; }

    /// <summary>
    /// Gets or sets track order of child item for serialization.
    /// </summary>
    [XmlAttribute("track")]
    [JsonIgnore]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int TrackSerialized
    {
        get { return Track ?? -1; }
        set { Track = value; }
    }

    /// <summary>
    /// Gets or sets release year of the child item.
    /// </summary>
    [XmlIgnore]
    public int? Year { get; set; }

    /// <summary>
    /// Gets or sets release year of the child item for serialization.
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
    [XmlIgnore]
    public long? Size { get; set; }

    /// <summary>
    /// Gets or sets filesize of the child item for serialization.
    /// </summary>
    [XmlAttribute("size")]
    [JsonIgnore]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public long SizeSerialized
    {
        get { return Size ?? -1; }
        set { Size = value; }
    }

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
    /// Gets or sets duration of the child item. In seconds.
    /// </summary>
    [XmlIgnore]
    public long? Duration { get; set; }

    /// <summary>
    /// Gets or sets duration of the child item for serialization.
    /// </summary>
    [XmlAttribute("duration")]
    [JsonIgnore]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public long DurationSerialized
    {
        get { return Duration ?? -1; }
        set { Duration = value; }
    }

    /// <summary>
    /// Gets or sets bitrate of the child item. In kbps.
    /// </summary>
    [XmlIgnore]
    public int? BitRate { get; set; }

    /// <summary>
    /// Gets or sets bitrate of the child item for serialization.
    /// </summary>
    [XmlAttribute("bitRate")]
    [JsonIgnore]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int BitRateSerialized
    {
        get { return BitRate ?? -1; }
        set { BitRate = value; }
    }

    /// <summary>
    /// Gets or sets path to the child item filename.
    /// </summary>
    [XmlAttribute("path")]
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets if child item is a video.
    /// </summary>
    [XmlIgnore]
    public bool? IsVideo { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether child item is a video for serialization.
    /// </summary>
    [XmlAttribute("isVideo")]
    [JsonIgnore]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsVideoSerialized
    {
        get { return IsVideo ?? false; }
        set { IsVideo = value; }
    }

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
    [XmlIgnore]
    public int? PlayCount { get; set; }

    /// <summary>
    /// Gets or sets child item play count for serialization.
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
    /// Gets or sets child item disc number.
    /// </summary>
    [XmlIgnore]
    public int? DiscNumber { get; set; }

    /// <summary>
    /// Gets or sets child item disc number for serialization.
    /// </summary>
    [XmlAttribute("discNumber")]
    [JsonIgnore]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int DiscNumberSerialized
    {
        get { return DiscNumber ?? -1; }
        set { DiscNumber = value; }
    }

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
    [XmlIgnore]
    public long? BookmarkPosition { get; set; }

    /// <summary>
    /// Gets or sets bookmark position in the child item for serialization.
    /// </summary>
    [XmlAttribute("bookmarkPosition")]
    [JsonIgnore]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public long BookmarkPositionSerialized
    {
        get { return BookmarkPosition ?? -1; }
        set { BookmarkPosition = value; }
    }

    /// <summary>
    /// Gets or sets original width of the child item.
    /// </summary>
    [XmlIgnore]
    public int? OriginalWidth { get; set; }

    /// <summary>
    /// Gets or sets original width of the child item for serialization.
    /// </summary>
    [XmlAttribute("originalWidth")]
    [JsonIgnore]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int OriginalWidthSerialized
    {
        get { return OriginalWidth ?? -1; }
        set { OriginalWidth = value; }
    }

    /// <summary>
    /// Gets or sets original height of the child item.
    /// </summary>
    [XmlIgnore]
    public int? OriginalHeight { get; set; }

    /// <summary>
    /// Gets or sets original height of the child item for serialization.
    /// </summary>
    [XmlAttribute("originalHeight")]
    [JsonIgnore]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int OriginalHeightSerialized
    {
        get { return OriginalHeight ?? -1; }
        set { OriginalHeight = value; }
    }

    /// <summary>
    /// Determines if track order of child item should be serialized.
    /// </summary>
    /// <returns>True if track order has a value.</returns>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeTrackSerialized() => Track != null;

    /// <summary>
    /// Determines if release year of child item should be serialized.
    /// </summary>
    /// <returns>True if release year has a value.</returns>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeYearSerialized() => Year != null;

    /// <summary>
    /// Determines if size of child item should be serialized.
    /// </summary>
    /// <returns>True if size has a value.</returns>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeSizeSerialized() => Size != null;

    /// <summary>
    /// Determines if duration of child item should be serialized.
    /// </summary>
    /// <returns>True if duration has a value.</returns>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeDurationSerialized() => Duration != null;

    /// <summary>
    /// Determines if bitrate of child item should be serialized.
    /// </summary>
    /// <returns>True if bitrate has a value.</returns>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeBitRateSerialized() => BitRate != null;

    /// <summary>
    /// Determines if child item is a video should be serialized.
    /// </summary>
    /// <returns>True if bitrate has a value.</returns>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeIsVideoSerialized() => IsVideo != null;

    /// <summary>
    /// Determines if play count of child item should be serialized.
    /// </summary>
    /// <returns>True if play count has a value.</returns>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializePlayCountSerialized() => PlayCount != null;

    /// <summary>
    /// Determines if disc number of child item should be serialized.
    /// </summary>
    /// <returns>True if disc number has a value.</returns>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeDiscNumberSerialized() => DiscNumber != null;

    /// <summary>
    /// Determines if bookmark position in child item should be serialized.
    /// </summary>
    /// <returns>True if bookmark position has a value.</returns>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeBookmarkPositionSerialized() => BookmarkPosition != null;

    /// <summary>
    /// Determines if original width of child item should be serialized.
    /// </summary>
    /// <returns>True if original width has a value.</returns>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeOriginalWidthSerialized() => OriginalWidth != null;

    /// <summary>
    /// Determines if original height of child item should be serialized.
    /// </summary>
    /// <returns>True if original height has a value.</returns>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeOriginalHeightSerialized() => OriginalHeight != null;
}
