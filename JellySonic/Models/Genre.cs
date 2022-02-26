using System.Xml.Serialization;

namespace JellySonic.Models;

/// <summary>
/// Subsonic Genre data type.
/// </summary>
public class Genre
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Genre"/> class.
    /// </summary>
    public Genre()
    {
        SongCount = string.Empty;
        AlbumCount = string.Empty;
    }

    /// <summary>
    /// Gets or sets song count of the genre.
    /// </summary>
    [XmlAttribute("songCount")]
    public string SongCount { get; set; }

    /// <summary>
    /// Gets or sets album count of the genre.
    /// </summary>
    [XmlAttribute("AlbumCount")]
    public string AlbumCount { get; set; }
}
