using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using static System.IO.Path;
using static JellySonic.Utils.Utils;

namespace JellySonic.Models;

/// <summary>
/// A Subsonic directory response data.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class DirectoryResponseData : ResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DirectoryResponseData"/> class.
    /// </summary>
    public DirectoryResponseData()
    {
        Id = string.Empty;
        ParentId = string.Empty;
        Name = string.Empty;
        Children = new List<Child>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectoryResponseData"/> class.
    /// </summary>
    /// <param name="folder">Folder item.</param>
    public DirectoryResponseData(Folder folder)
    {
        Id = folder.Id.ToString();
        ParentId = folder.ParentId.ToString();
        Name = folder.Name;
        Children = folder.Children.Select(child => new Child((Folder)child)).ToList();
    }

    /// <summary>
    /// Gets or sets ID of the directory.
    /// </summary>
    [XmlAttribute("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets parent ID of the directory.
    /// </summary>
    [XmlAttribute("parent")]
    public string ParentId { get; set; }

    /// <summary>
    /// Gets or sets name of the directory.
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets date of starring the directory.
    /// </summary>
    [XmlAttribute("starred")]
    public DateTime Starred { get; set; }

    /// <summary>
    /// Gets or sets collection of directory child elements.
    /// </summary>
    [XmlIgnore]
    public IEnumerable<Child> Children { get; set; }

    /// <summary>
    /// Gets or sets collection of directory child elements used for serialization.
    /// </summary>
    [XmlElement("child")]
    public List<Child> ChildrenSurrogate
    {
        get { return Children.ToList(); }
        set { Children = value; }
    }
}

/// <summary>
/// A child of Subsonic directory response data.
/// </summary>
[XmlRoot("child")]
public class Child
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Child"/> class.
    /// </summary>
    public Child()
    {
        Id = string.Empty;
        ParentId = string.Empty;
        Title = string.Empty;
        IsDir = string.Empty;
        AlbumName = string.Empty;
        ArtistName = string.Empty;
        TrackOrder = string.Empty;
        ReleaseYear = string.Empty;
        Genre = string.Empty;
        CoverArt = string.Empty;
        FileSize = "0";
        ContentType = string.Empty;
        FilenameSuffix = string.Empty;
        TranscodedContentType = string.Empty;
        TranscodedSuffix = string.Empty;
        Duration = string.Empty;
        BitRate = "0";
        Path = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Child"/> class.
    /// </summary>
    /// <param name="item">General item.</param>
    public Child(BaseItem item)
    {
        Id = item.Id.ToString();
        ParentId = item.ParentId.ToString();
        Title = item.Name;
        IsDir = item.IsFolder.ToString();
        AlbumName = item.Album;

        try
        {
            ArtistName = ((Audio)item).AlbumArtists.FirstOrDefault(string.Empty);
        }
        catch
        {
            ArtistName = string.Empty;
        }

        TrackOrder = item.IndexNumber.ToString() ?? "0";
        ReleaseYear = item.ProductionYear.ToString() ?? "0";
        Genre = item.Genres.FirstOrDefault(string.Empty);
        CoverArt = string.Empty;
        FileSize = item.Size?.ToString(NumberFormatInfo.InvariantInfo) ?? "0";
        ContentType = string.Empty;

        var ext = GetExtension(item.Path);
        FilenameSuffix = string.IsNullOrEmpty(ext) ? string.Empty : ext[1..];

        TranscodedContentType = string.Empty;
        TranscodedSuffix = string.Empty;
        Duration = TicksToSeconds(item.RunTimeTicks).ToString(NumberFormatInfo.InvariantInfo);
        BitRate = "0";
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
    public string ParentId { get; set; }

    /// <summary>
    /// Gets or sets name of the child item.
    /// </summary>
    [XmlAttribute("title")]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets if this child item is a directory.
    /// </summary>
    [XmlAttribute("isDir")]
    public string IsDir { get; set; }

    /// <summary>
    /// Gets or sets album name of the child item.
    /// </summary>
    [XmlAttribute("album")]
    public string AlbumName { get; set; }

    /// <summary>
    /// Gets or sets artist name of the child item.
    /// </summary>
    [XmlAttribute("artist")]
    public string ArtistName { get; set; }

    /// <summary>
    /// Gets or sets track order of child.
    /// </summary>
    [XmlAttribute("track")]
    public string TrackOrder { get; set; }

    /// <summary>
    /// Gets or sets release year of the child - well assuming audio.
    /// </summary>
    [XmlAttribute("year")]
    public string ReleaseYear { get; set; }

    /// <summary>
    /// Gets or sets genre of the child.
    /// </summary>
    [XmlAttribute("genre")]
    public string Genre { get; set; }

    /// <summary>
    /// Gets or sets cover art of the child item.
    /// </summary>
    [XmlAttribute("coverArt")]
    public string CoverArt { get; set; }

    /// <summary>
    /// Gets or sets filesize of the child item.
    /// </summary>
    [XmlAttribute("size")]
    public string FileSize { get; set; }

    /// <summary>
    /// Gets or sets content type of the child item.
    /// TODO - probably audio/something.
    /// </summary>
    [XmlAttribute("contentType")]
    public string ContentType { get; set; }

    /// <summary>
    /// Gets or sets filename suffix of the child item.
    /// </summary>
    [XmlAttribute("suffix")]
    public string FilenameSuffix { get; set; }

    /// <summary>
    /// Gets or sets transcoded content type of the child item.
    /// </summary>
    [XmlAttribute("transcodedContentType")]
    public string TranscodedContentType { get; set; }

    /// <summary>
    /// Gets or sets transcoded filename suffix of the child item.
    /// </summary>
    [XmlAttribute("transcodedSuffix")]
    public string TranscodedSuffix { get; set; }

    /// <summary>
    /// Gets or sets duration of the child item.
    /// </summary>
    [XmlAttribute("duration")]
    public string Duration { get; set; }

    /// <summary>
    /// Gets or sets bitrate of the child item.
    /// </summary>
    [XmlAttribute("bitRate")]
    public string BitRate { get; set; }

    /// <summary>
    /// Gets or sets path to the child item filename.
    /// </summary>
    [XmlAttribute("path")]
    public string Path { get; set; }
}
