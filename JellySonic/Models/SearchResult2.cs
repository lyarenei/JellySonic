using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using JellySonic.Types;
using MediaBrowser.Controller.Entities;

namespace JellySonic.Models;

/// <summary>
/// A Subsonic Search Result 2 data type.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class SearchResult2 : IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchResult2"/> class.
    /// </summary>
    public SearchResult2()
    {
        Artists = new List<Artist>();
        Albums = new List<Child>();
        Songs = new List<Child>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchResult2"/> class.
    /// </summary>
    /// <param name="artists">List of artist items.</param>
    /// <param name="albums">List of album items.</param>
    /// <param name="songs">List of song items.</param>
    public SearchResult2(IEnumerable<BaseItem> artists, IEnumerable<BaseItem> albums, IEnumerable<BaseItem> songs)
    {
        Artists = artists.Select(artist => new Artist(artist));
        Albums = albums.Select(album => new Child(album));
        Songs = songs.Select(song => new Child(song));
    }

    /// <summary>
    /// Gets or sets artists.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<Artist> Artists { get; set; }

    /// <summary>
    /// Gets or sets albums.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<Child> Albums { get; set; }

    /// <summary>
    /// Gets or sets songs.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<Child> Songs { get; set; }

    /// <summary>
    /// Gets or sets artists used for serialization.
    /// <seealso cref="Artists"/>
    /// </summary>
    [XmlElement("artist")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<Artist> ArtistsSerialize
    {
        get { return Artists.ToList(); }
        set { Artists = value; }
    }

    /// <summary>
    /// Gets or sets albums used for serialization.
    /// <seealso cref="Albums"/>
    /// </summary>
    [XmlElement("album")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<Child> AlbumsSerialize
    {
        get { return Albums.ToList(); }
        set { Albums = value; }
    }

    /// <summary>
    /// Gets or sets songs used for serialization.
    /// <seealso cref="Songs"/>
    /// </summary>
    [XmlElement("song")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<Child> SongsSerialize
    {
        get { return Songs.ToList(); }
        set { Songs = value; }
    }
}
