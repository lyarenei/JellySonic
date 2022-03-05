using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using JellySonic.Types;
using MediaBrowser.Controller.Entities;

namespace JellySonic.Models;

/// <summary>
/// A Subsonic directory response data.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class SubsonicDirectory : IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubsonicDirectory"/> class.
    /// </summary>
    public SubsonicDirectory()
    {
        ChildList = new List<Child>();
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
            ChildList = ((Folder)item).Children.Select(childItem => new Child(childItem)).ToList();
        }
        catch
        {
            ChildList = new List<Child>();
        }

        Id = item.Id.ToString();
        Parent = item.ParentId.ToString();
        Name = item.Name;
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
    /// Gets or sets collection of directory child elements.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<Child> ChildList { get; set; }

    /// <summary>
    /// Gets or sets collection of directory child elements used for serialization.
    /// </summary>
    [XmlElement("child")]
    [JsonPropertyName("child")]
    public List<Child> ChildListSerialize
    {
        get { return ChildList.ToList(); }
        set { ChildList = value; }
    }
}
