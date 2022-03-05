using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using JellySonic.Types;
using MediaBrowser.Controller.Entities;

namespace JellySonic.Models;

/// <summary>
/// Subsonic AlbumList data type.
/// </summary>
public class AlbumList : IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumList"/> class.
    /// </summary>
    public AlbumList()
    {
        Albums = new Collection<Child>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumList"/> class.
    /// </summary>
    /// <param name="items">Collection of items.</param>
    public AlbumList(IEnumerable<BaseItem> items)
    {
        Albums = new Collection<Child>(items.Select(item => new Child(item)).ToList());
    }

    /// <summary>
    /// Gets list of albums.
    /// </summary>
    [XmlElement("album")]
    [JsonPropertyName("album")]
    public Collection<Child> Albums { get; }
}
