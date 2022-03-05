using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using JellySonic.Types;
using MediaBrowser.Controller.Entities;

namespace JellySonic.Models;

/// <summary>
/// Subsonic AlbumList2 data type.
/// </summary>
public class AlbumList2 : IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumList2"/> class.
    /// </summary>
    public AlbumList2()
    {
        Albums = new Collection<AlbumId3>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumList2"/> class.
    /// </summary>
    /// <param name="items">Collection of items.</param>
    public AlbumList2(IEnumerable<BaseItem> items)
    {
        Albums = new Collection<AlbumId3>(items.Select(item => new AlbumId3(item)).ToList());
    }

    /// <summary>
    /// Gets list of albums.
    /// </summary>
    [XmlElement("album")]
    [JsonPropertyName("album")]
    public Collection<AlbumId3> Albums { get; }
}
