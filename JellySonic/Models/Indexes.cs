using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using MediaBrowser.Controller.Entities;

namespace JellySonic.Models;

/// <summary>
/// A Subsonic Indexes data type.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class Indexes
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Indexes"/> class.
    /// </summary>
    public Indexes()
    {
        /* todo: IndexList can contain multiple types
         <xs:sequence>
            <xs:element name="shortcut" type="sub:Artist" minOccurs="0" maxOccurs="unbounded"/>
            <xs:element name="index" type="sub:Index" minOccurs="0" maxOccurs="unbounded"/>
            <xs:element name="child" type="sub:Child" minOccurs="0" maxOccurs="unbounded"/> <!-- Added in 1.7.0 -->
        </xs:sequence>
        */
        LastModified = DateTime.Now.ToString(Utils.Utils.IsoDateFormat, DateTimeFormatInfo.InvariantInfo);
        IgnoredArticles = string.Empty;
        IndexList = new List<Index>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Indexes"/> class.
    /// </summary>
    /// <param name="artists">List of items from query.</param>
    public Indexes(IEnumerable<BaseItem> artists)
    {
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
    public IEnumerable<Index> IndexList { get; set; }

    /// <summary>
    /// Gets or sets index of indexes used for serialization.
    /// <seealso cref="IndexList"/>
    /// </summary>
    [XmlElement("index")]
    [JsonPropertyName("index")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<Index> IndexListSurrogate
    {
        get { return IndexList.ToList(); }
        set { IndexList = value; }
    }
}
