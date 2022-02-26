using System.Xml.Serialization;
using MediaBrowser.Controller.Entities;

namespace JellySonic.Models;

/// <summary>
/// Subsonic Artist data type.
/// </summary>
public class Artist
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Artist"/> class.
    /// </summary>
    public Artist()
    {
        Id = string.Empty;
        Name = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Artist"/> class.
    /// </summary>
    /// <param name="item">Artist item.</param>
    public Artist(BaseItem item)
    {
        Id = item.Id.ToString();
        Name = item.Name;
    }

    /// <summary>
    /// Gets or sets artist ID.
    /// </summary>
    [XmlAttribute("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets artist name.
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets URL to image of the artist.
    /// </summary>
    [XmlAttribute("artistImageUrl")]
    public string? ArtistImageUrl { get; set; }

    /// <summary>
    /// Gets or sets date of starred artist.
    /// </summary>
    [XmlAttribute("starred")]
    public string? Starred { get; set; }

    /// <summary>
    /// Gets or sets artist user rating.
    /// </summary>
    [XmlAttribute("userRating")]
    public string? UserRating { get; set; }

    /// <summary>
    /// Gets or sets artist average rating.
    /// </summary>
    [XmlAttribute("averageRating")]
    public string? AverageRating { get; set; }
}
