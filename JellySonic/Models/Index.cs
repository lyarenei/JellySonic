using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using JellySonic.Types;

namespace JellySonic.Models;

/// <summary>
/// Subsonic Index data type.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class Index : IIndexItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Index"/> class.
    /// </summary>
    public Index()
    {
        Artists = new List<Artist>();
        Name = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Index"/> class.
    /// </summary>
    /// <param name="name">Name of the index.</param>
    public Index(string name)
    {
        Name = name;
        Artists = new List<Artist>();
    }

    /// <summary>
    /// Gets or sets name of the index.
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets artists in the index.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<Artist> Artists { get; set; }

    /// <summary>
    /// Gets or sets artists in the index used for serialization.
    /// <seealso cref="Artists"/>
    /// </summary>
    [XmlElement("artist")]
    [JsonPropertyName("artist")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<Artist> ArtistsSerialize
    {
        get { return Artists.ToList(); }
        set { Artists = value; }
    }
}
