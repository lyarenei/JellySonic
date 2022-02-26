using System.Xml.Serialization;

namespace JellySonic.Models;

/// <summary>
/// Subsonic NowPlayingEntry data type.
/// </summary>
public class NowPlayingEntry : Child
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NowPlayingEntry"/> class.
    /// </summary>
    public NowPlayingEntry()
    {
        Username = string.Empty;
        MinutesAgo = string.Empty;
        PlayerId = string.Empty;
    }

    /// <summary>
    /// Gets or sets username.
    /// </summary>
    [XmlAttribute("username")]
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets how many minutes entry started playing.
    /// </summary>
    [XmlAttribute("minutesAgo")]
    public string MinutesAgo { get; set; }

    /// <summary>
    /// Gets or sets the player ID.
    /// </summary>
    [XmlAttribute("playerId")]
    public string PlayerId { get; set; }

    /// <summary>
    /// Gets or sets the name of the player.
    /// </summary>
    [XmlAttribute("palyerName")]
    public string? PlayerName { get; set; }
}
