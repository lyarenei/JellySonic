using System;
using System.Globalization;
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
        AlbumCount = albumCount.ToString(NumberFormatInfo.InvariantInfo);
        SongCount = songCount.ToString(NumberFormatInfo.InvariantInfo);
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
        var realValue = Convert.ToInt32(SongCount, NumberFormatInfo.InvariantInfo);
        realValue += value;
        SongCount = realValue.ToString(NumberFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Increment album count with a specified value.
    /// </summary>
    /// <param name="value">Value to increment with.</param>
    public void IncrementAlbumCount(int value = 1)
    {
        var realValue = Convert.ToInt32(AlbumCount, NumberFormatInfo.InvariantInfo);
        realValue += value;
        AlbumCount = realValue.ToString(NumberFormatInfo.InvariantInfo);
    }
}
