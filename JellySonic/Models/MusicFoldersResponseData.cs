using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;
using MediaBrowser.Controller.Entities;

namespace JellySonic.Models;

/// <summary>
/// A Subsonic music folders response data.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class MusicFoldersResponseData : ResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MusicFoldersResponseData"/> class.
    /// </summary>
    public MusicFoldersResponseData()
    {
        MusicFolders = new List<MusicFolder>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MusicFoldersResponseData"/> class.
    /// </summary>
    /// <param name="folders">List of folders.</param>
    public MusicFoldersResponseData(IEnumerable<Folder> folders)
    {
        var musicFolders = new List<MusicFolder>();
        foreach (var folder in folders)
        {
            musicFolders.Add(new MusicFolder(folder));
        }

        MusicFolders = musicFolders;
    }

    /// <summary>
    /// Gets or sets music folders.
    /// </summary>
    [XmlIgnore]
    public IEnumerable<MusicFolder> MusicFolders { get; set; }

    /// <summary>
    /// Gets or sets list of music folders used for serialization.
    /// </summary>
    [XmlElement("musicFolder")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<MusicFolder> MusicFoldersSurrogate
    {
        get { return MusicFolders.ToList(); }
        set { MusicFolders = value; }
    }
}

/// <summary>
/// Subsonic music folder.
/// </summary>
[XmlRoot("musicFolder")]
public class MusicFolder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MusicFolder"/> class.
    /// </summary>
    public MusicFolder()
    {
        Id = string.Empty;
        Name = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MusicFolder"/> class.
    /// </summary>
    /// <param name="folder">Folder item.</param>
    public MusicFolder(Folder folder)
    {
        Id = folder.Id.ToString();
        Name = folder.Name;
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
    public string Name { get; set; }
}
