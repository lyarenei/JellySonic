using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;
using JellySonic.Types;
using MediaBrowser.Controller.Entities;

namespace JellySonic.Models;

/// <summary>
/// Subsonic AlbumList data type.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class AlbumList : IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumList"/> class.
    /// </summary>
    public AlbumList()
    {
        Albums = new List<Child>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumList"/> class.
    /// </summary>
    /// <param name="items">Collection of items.</param>
    public AlbumList(IEnumerable<BaseItem> items)
    {
        Albums = items.Select(item => new Child(item));
    }

    /// <summary>
    /// Gets or sets list of albums.
    /// </summary>
    [XmlIgnore]
    public IEnumerable<Child> Albums { get; set; }

    /// <summary>
    /// Gets or sets list of albums for serialization.
    /// </summary>
    [XmlElement("album")]
    public List<Child> AlbumsSerialize
    {
        get { return Albums.ToList(); }
        set { Albums = value; }
    }
}
