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
        SongCount = 0;
        AlbumCount = 0;
        Name = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Genre"/> class.
    /// </summary>
    /// <param name="name">Genre name.</param>
    /// <param name="albumCount">Genre album count.</param>
    /// <param name="songCount">Genre song count.</param>
    public Genre(string name, int albumCount, int songCount)
    {
        Name = name;
        AlbumCount = albumCount;
        SongCount = songCount;
    }

    /// <summary>
    /// Gets or sets song count of the genre.
    /// </summary>
    [XmlAttribute("songCount")]
    public int SongCount { get; set;  }

    /// <summary>
    /// Gets or sets album count of the genre.
    /// </summary>
    [XmlAttribute("AlbumCount")]
    public int AlbumCount { get; set; }

    /// <summary>
    /// Gets or sets genre name.
    /// </summary>
    [XmlText]
    public string Name { get; set; }

    /// <summary>
    /// Increment song count with a specified value.
    /// </summary>
    /// <param name="value">Value to increment with.</param>
    public void IncrementSongCount(int value = 1)
    {
        SongCount += value;
    }

    /// <summary>
    /// Increment album count with a specified value.
    /// </summary>
    /// <param name="value">Value to increment with.</param>
    public void IncrementAlbumCount(int value = 1)
    {
        AlbumCount += value;
    }
}
