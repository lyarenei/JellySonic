using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace JellySonic.Models;

/// <summary>
/// Subsonic IndexID3 data type.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class IndexId3
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IndexId3"/> class.
    /// </summary>
    public IndexId3()
    {
        Artists = new List<ArtistId3>();
        Name = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexId3"/> class.
    /// </summary>
    /// <param name="name">Index name.</param>
    public IndexId3(string name)
    {
        Artists = new List<ArtistId3>();
        Name = name;
    }

    /// <summary>
    /// Gets or sets collection of artists.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<ArtistId3> Artists { get; set; }

    /// <summary>
    /// Gets or sets collection of artists for serialization.
    /// </summary>
    [XmlElement("artist")]
    [JsonPropertyName("artist")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<ArtistId3> ArtistsSerialize
    {
        get { return Artists.ToList(); }
        set { Artists = value; }
    }

    /// <summary>
    /// Gets or sets index name.
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; }
}
