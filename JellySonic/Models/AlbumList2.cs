using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;
using JellySonic.Types;
using MediaBrowser.Controller.Entities;

namespace JellySonic.Models;

/// <summary>
/// Subsonic AlbumList2 data type.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class AlbumList2 : IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumList2"/> class.
    /// </summary>
    public AlbumList2()
    {
        Albums = new List<AlbumId3>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumList2"/> class.
    /// </summary>
    /// <param name="items">Collection of items.</param>
    public AlbumList2(IEnumerable<BaseItem> items)
    {
        Albums = items.Select(item => new AlbumId3(item));
    }

    /// <summary>
    /// Gets or sets list of albums.
    /// </summary>
    [XmlIgnore]
    public IEnumerable<AlbumId3> Albums { get; set; }

    /// <summary>
    /// Gets or sets list of albums for serialization.
    /// </summary>
    [XmlElement("album")]
    public List<AlbumId3> AlbumsSerialize
    {
        get { return Albums.ToList(); }
        set { Albums = value; }
    }
}
