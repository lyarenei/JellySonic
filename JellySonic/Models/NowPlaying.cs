using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace JellySonic.Models;

/// <summary>
/// Subsonic NowPlaying data type.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class NowPlaying
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NowPlaying"/> class.
    /// </summary>
    public NowPlaying()
    {
        Entries = new List<NowPlayingEntry>();
    }

    /// <summary>
    /// Gets or sets collection of now playing entries.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<NowPlayingEntry> Entries { get; set; }

    /// <summary>
    /// Gets or sets collection of now playing entries for serialization.
    /// </summary>
    [XmlElement("entry")]
    [JsonPropertyName("entry")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<NowPlayingEntry> EntriesSerialize
    {
        get { return Entries.ToList(); }
        set { Entries = value; }
    }
}
