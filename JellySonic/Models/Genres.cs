using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;

namespace JellySonic.Models;

/// <summary>
/// Subsonic Genres data type.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class Genres
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Genres"/> class.
    /// </summary>
    public Genres()
    {
        GenreList = new List<Genre>();
    }

    /// <summary>
    /// Gets or sets collection of genres.
    /// </summary>
    public IEnumerable<Genre> GenreList { get; set; }

    /// <summary>
    /// Gets or sets collection of genres for serialization.
    /// </summary>
    [XmlElement("genre")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<Genre> GenreListSerialize
    {
        get { return GenreList.ToList(); }
        set { GenreList = value; }
    }
}
