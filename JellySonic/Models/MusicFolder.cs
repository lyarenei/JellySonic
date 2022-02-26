using System.Xml.Serialization;
using MediaBrowser.Controller.Entities;

namespace JellySonic.Models;

/// <summary>
/// Subsonic MusicFolder data type.
/// </summary>
public class MusicFolder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MusicFolder"/> class.
    /// </summary>
    public MusicFolder()
    {
        Id = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MusicFolder"/> class.
    /// </summary>
    /// <param name="item">Folder item.</param>
    public MusicFolder(BaseItem item)
    {
        Id = item.Id.ToString();
        Name = item.Name;
    }

    /// <summary>
    /// Gets or sets ID of the folder.
    /// </summary>
    [XmlAttribute("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets name of the folder.
    /// </summary>
    [XmlAttribute("name")]
    public string? Name { get; set; }
}
