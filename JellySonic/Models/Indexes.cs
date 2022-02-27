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
        IndexList = new List<IIndexItem>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Indexes"/> class.
    /// </summary>
    /// <param name="artists">List of items from query.</param>
    public Indexes(IEnumerable<BaseItem> artists)
    {
        // TODO this is incorrect implementation
        LastModified = DateTime.Now.ToString(Utils.Utils.IsoDateFormat, DateTimeFormatInfo.InvariantInfo);
        IgnoredArticles = string.Empty;
        var artistIndex = new List<Index>();

        foreach (var artist in artists)
        {
            string indexName = char.IsLetter(artist.SortName.First()) ? artist.SortName.First().ToString() : "#";
            if (!artistIndex.Exists(idx => idx.Name == indexName))
            {
                artistIndex.Add(new Index(indexName));
            }

            int aIdx = artistIndex.FindIndex(idx => idx.Name == indexName);
            artistIndex[aIdx].Artists = artistIndex[aIdx].Artists.Append(new Artist(artist));
        }

        IndexList = artistIndex;
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
    /// Gets or sets index of indexes containing artists.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<IIndexItem> IndexList { get; set; }

    /// <summary>
    /// Gets or sets index of indexes used for serialization.
    /// <seealso cref="IndexList"/>
    /// </summary>
    [XmlElement("shortcut", typeof(Artist))]
    [XmlElement("index", typeof(Index))]
    [XmlElement("child", typeof(Child))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<object> IndexListSerialize
    {
        get { return IndexList.Cast<object>().ToList(); }
        set { IndexList = value.Cast<IIndexItem>(); }
    }
}
