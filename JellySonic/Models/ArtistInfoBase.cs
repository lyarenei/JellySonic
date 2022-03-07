using System.Xml.Serialization;

namespace JellySonic.Models;

/// <summary>
/// A Subsonic ArtistInfoBase data type.
/// </summary>
public class ArtistInfoBase
{
    /// <summary>
    /// Gets or sets bio.
    /// </summary>
    [XmlElement("biography")]
    public string? Biography { get; set; }

    /// <summary>
    /// Gets or sets MusicBrainz ID.
    /// </summary>
    [XmlElement("musicBrainzId")]
    public string? MusicBrainzId { get; set; }

    /// <summary>
    /// Gets or sets LastFM URL.
    /// </summary>
    [XmlElement("lastFmUrl")]
    public string? LastFmUrl { get; set; }

    /// <summary>
    /// Gets or sets small image URL.
    /// </summary>
    [XmlElement("smallImageUrl")]
    public string? SmallImageUrl { get; set; }

    /// <summary>
    /// Gets or sets medium image URL.
    /// </summary>
    [XmlElement("mediumImageUrl")]
    public string? MediumImageUrl { get; set; }

    /// <summary>
    /// Gets or sets large image URL.
    /// </summary>
    [XmlElement("largeImageUrl")]
    public string? LargeImageUrl { get; set; }
}
