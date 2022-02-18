using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Dto;

namespace JellySonic.Models;

/// <summary>
/// A Subsonic Artists response data.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class ArtistsResponseData : ResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistsResponseData"/> class.
    /// </summary>
    public ArtistsResponseData()
    {
        IgnoredArticles = string.Empty;
        Index = new List<ArtistIndex>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistsResponseData"/> class.
    /// </summary>
    /// <param name="items">List of items from query.</param>
    public ArtistsResponseData(IEnumerable<(BaseItem Item, ItemCounts ItemCounts)> items)
    {
        IgnoredArticles = string.Empty;
        var artistIndex = new List<ArtistIndex>();

        foreach (var (item, _) in items)
        {
            string indexName = char.IsLetter(item.Name.First()) ? item.Name.First().ToString() : "#";
            if (!artistIndex.Exists(idx => idx.Name == indexName))
            {
                artistIndex.Add(new ArtistIndex(indexName));
            }

            int aIdx = artistIndex.FindIndex(idx => idx.Name == indexName);
            artistIndex[aIdx].Artists = artistIndex[aIdx].Artists.Append(new IndexArtist(item));
        }

        Index = artistIndex;
    }

    /// <summary>
    /// Gets or sets a series of ignored articles; separated by space.
    /// </summary>
    [XmlAttribute("ignoredArticles")]
    public string IgnoredArticles { get; set; }

    /// <summary>
    /// Gets or sets index of indexes containing artists.
    /// </summary>
    [XmlIgnore]
    public IEnumerable<ArtistIndex> Index { get; set; }

    /// <summary>
    /// Gets or sets index of indexes used for serialization.
    /// <seealso cref="Index"/>
    /// </summary>
    [XmlElement("index")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<ArtistIndex> IndexSurrogate
    {
        get { return Index.ToList(); }
        set { Index = value; }
    }
}

/// <summary>
/// An artist index used to hold <see cref="IndexArtist"/> in <see cref="Artists"/> response data.
/// </summary>
[XmlRoot("index")]
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class ArtistIndex
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistIndex"/> class.
    /// </summary>
    public ArtistIndex()
    {
        Artists = new List<IndexArtist>();
        Name = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistIndex"/> class.
    /// </summary>
    /// <param name="name">Name of the index.</param>
    public ArtistIndex(string name)
    {
        Name = name;
        Artists = new List<IndexArtist>();
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
    public IEnumerable<IndexArtist> Artists { get; set; }

    /// <summary>
    /// Gets or sets artists in the index used for serialization.
    /// <seealso cref="Artists"/>
    /// </summary>
    [XmlElement("artist")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<IndexArtist> ArtistsSurrogate
    {
        get { return Artists.ToList(); }
        set { Artists = value; }
    }
}

/// <summary>
/// An Artist class used to hold data in <see cref="ArtistsResponseData"/> response data.
/// </summary>
[XmlRoot("artist")]
public class IndexArtist
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IndexArtist"/> class.
    /// </summary>
    public IndexArtist()
    {
        AlbumCount = "0";
        CoverArt = string.Empty;
        Id = string.Empty;
        Name = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexArtist"/> class.
    /// </summary>
    /// <param name="item">A <see cref="BaseItem"/> item.</param>
    public IndexArtist(BaseItem? item)
    {
        AlbumCount = "0";
        CoverArt = string.Empty;
        Id = item != null ? item.Id.ToString() : string.Empty;
        Name = item != null ? item.Name : string.Empty;
    }

    /// <summary>
    /// Gets or sets artist album count.
    /// </summary>
    [XmlAttribute("albumCount")]
    public string AlbumCount { get; set; }

    /// <summary>
    /// Gets or sets artist cover art.
    /// </summary>
    [XmlAttribute("coverArt")]
    public string CoverArt { get; set; }

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
}
