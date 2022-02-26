using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using JellySonic.Types;
using MediaBrowser.Controller.Entities;

namespace JellySonic.Models;

/// <summary>
/// Subsonic MusicFolders data type.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class MusicFolders : IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Models.MusicFolders"/> class.
    /// </summary>
    public MusicFolders()
    {
        Folders = new List<MusicFolder>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MusicFolders"/> class.
    /// </summary>
    /// <param name="items">Collection of folder items.</param>
    public MusicFolders(IEnumerable<BaseItem> items)
    {
        Folders = items.Select(item => new MusicFolder(item)).ToList();
    }

    /// <summary>
    /// Gets or sets a collection of music folders.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<MusicFolder> Folders { get; set; }

    /// <summary>
    /// Gets or sets collection of music folders for serialization.
    /// </summary>
    [XmlElement("musicFolder")]
    [JsonPropertyName("musicFolder")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<MusicFolder> GenreListSerialize
    {
        get { return Folders.ToList(); }
        set { Folders = value; }
    }
}
