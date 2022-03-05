using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using JellySonic.Types;
using MediaBrowser.Controller.Entities;

namespace JellySonic.Models;

/// <summary>
/// Subsonic MusicFolders data type.
/// </summary>
public class MusicFolders : IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MusicFolders"/> class.
    /// </summary>
    public MusicFolders()
    {
        Folders = new Collection<MusicFolder>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MusicFolders"/> class.
    /// </summary>
    /// <param name="items">Collection of folder items.</param>
    public MusicFolders(IEnumerable<BaseItem> items)
    {
        Folders = new Collection<MusicFolder>(items.Select(item => new MusicFolder(item)).ToList());
    }

    /// <summary>
    /// Gets a collection of music folders.
    /// </summary>
    [XmlElement("musicFolder")]
    [JsonPropertyName("musicFolder")]
    public Collection<MusicFolder> Folders { get; }
}
