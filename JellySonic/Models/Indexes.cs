using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using JellySonic.Types;
using MediaBrowser.Controller.Entities;

namespace JellySonic.Models;

/// <summary>
/// A Subsonic Indexes data type.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class Indexes : IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Indexes"/> class.
    /// </summary>
    public Indexes()
    {
        LastModified = DateTime.Now.ToString(Utils.Utils.IsoDateFormat, DateTimeFormatInfo.InvariantInfo);
        IgnoredArticles = string.Empty;
        IndexArtists = new List<Artist>();
        IndexList = new List<Index>();
        IndexChildList = new List<Child>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Indexes"/> class.
    /// </summary>
    /// <param name="artists">List of artist items from query.</param>
    /// <param name="songs">List of song items from query.</param>
    public Indexes(IEnumerable<BaseItem> artists, IEnumerable<BaseItem> songs)
    {
        LastModified = DateTime.Now.ToString(Utils.Utils.IsoDateFormat, DateTimeFormatInfo.InvariantInfo);
        IgnoredArticles = string.Empty;
        IndexArtists = artists.Select(artist => new Artist(artist));
        IndexChildList = songs.Select(song => new Child(song, parentIsArtist: true));

        var index = new List<Index>();
        foreach (var artist in artists)
        {
            string indexName = GetIndexKey(artist);
            if (!index.Exists(idx => idx.Name == indexName))
            {
                index.Add(new Index(indexName));
            }

            int aIdx = index.FindIndex(idx => idx.Name == indexName);
            index[aIdx].Artists = index[aIdx].Artists.Append(new Artist(artist));
        }

        IndexList = index;
    }

    /// <summary>
    /// Gets or sets last modified time.
    /// </summary>
    [XmlAttribute("lastModified")]
    public string LastModified { get; set; }

    /// <summary>
    /// Gets or sets a series of ignored articles; separated by space.
    /// </summary>
    [XmlAttribute("ignoredArticles")]
    public string IgnoredArticles { get; set; }

    /// <summary>
    /// Gets or sets index of artists.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<Artist> IndexArtists { get; set; }

    /// <summary>
    /// Gets or sets index of artists used for serialization.
    /// <seealso cref="IndexArtists"/>
    /// </summary>
    [XmlElement("shortcut")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<Artist> IndexArtistsSerialize
    {
        get { return IndexArtists.ToList(); }
        set { IndexArtists = value; }
    }

    /// <summary>
    /// Gets or sets indexes.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<Index> IndexList { get; set; }

    /// <summary>
    /// Gets or sets indexes used for serialization.
    /// <seealso cref="IndexList"/>
    /// </summary>
    [XmlElement("index")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<Index> IndexesSerialize
    {
        get { return IndexList.ToList(); }
        set { IndexList = value; }
    }

    /// <summary>
    /// Gets or sets index of artists.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<Child> IndexChildList { get; set; }

    /// <summary>
    /// Gets or sets index of artists used for serialization.
    /// <seealso cref="IndexArtists"/>
    /// </summary>
    [XmlElement("child")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<Child> IndexChildListSerialize
    {
        get { return IndexChildList.ToList(); }
        set { IndexChildList = value; }
    }

    private static string GetIndexKey(BaseItem item)
    {
        return char.IsLetter(item.SortName.First()) ? item.SortName.First().ToString() : "#";
    }
}
