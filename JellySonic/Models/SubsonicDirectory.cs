using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using JellySonic.Types;
using MediaBrowser.Controller.Entities;

namespace JellySonic.Models;

/// <summary>
/// A Subsonic directory response data.
/// </summary>
public class SubsonicDirectory : IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubsonicDirectory"/> class.
    /// </summary>
    public SubsonicDirectory()
    {
        ChildCollection = new Collection<Child>();
        Id = string.Empty;
        Name = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubsonicDirectory"/> class.
    /// </summary>
    /// <param name="item">Folder item.</param>
    public SubsonicDirectory(BaseItem item)
    {
        try
        {
            var folder = (Folder)item;
            var childList = folder.Children.Select(childItem => new Child(childItem)).ToList();
            ChildCollection = new Collection<Child>(childList);
        }
        catch
        {
            ChildCollection = new Collection<Child>();
        }

        Id = item.Id.ToString();
        Parent = item.ParentId.ToString();
        Name = item.Name;
    }

    /// <summary>
    /// Gets or sets iD of the directory.
    /// </summary>
    [XmlAttribute("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets parent ID of the directory.
    /// </summary>
    [XmlAttribute("parent")]
    public string? Parent { get; set; }

    /// <summary>
    /// Gets or sets name of the directory.
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets date of starring the directory.
    /// </summary>
    [XmlAttribute("starred")]
    public string? Starred { get; set; }

    /// <summary>
    /// Gets or sets directory user rating.
    /// </summary>
    [XmlAttribute("userRating")]
    public string? UserRating { get; set; }

    /// <summary>
    /// Gets or sets directory average rating.
    /// </summary>
    [XmlAttribute("averageRating")]
    public string? AverageRating { get; set; }

    /// <summary>
    /// Gets or sets directory play count.
    /// </summary>
    [XmlAttribute("playCount")]
    public string? PlayCount { get; set; }

    /// <summary>
    /// Gets collection of directory child elements.
    /// </summary>
    [XmlElement("child")]
    [JsonPropertyName("child")]
    public Collection<Child> ChildCollection { get; }
}
