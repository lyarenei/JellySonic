using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
public class Indexes : IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Indexes"/> class.
    /// </summary>
    public Indexes()
    {
        LastModified = DateTime.Now.ToString(Utils.Utils.IsoDateFormat, DateTimeFormatInfo.InvariantInfo);
        IgnoredArticles = string.Empty;
        Artists = new Collection<Artist>();
        IndexCollection = new Collection<Index>();
        ChildCollection = new Collection<Child>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Indexes"/> class.
    /// </summary>
    /// <param name="artists">List of artist items from query.</param>
    /// <param name="songs">List of song items from query.</param>
    public Indexes(IEnumerable<BaseItem> artists, IEnumerable<BaseItem> songs)
    {
        var artistList = artists.ToList();
        var songList = songs.ToList();

        LastModified = DateTime.Now.ToString(Utils.Utils.IsoDateFormat, DateTimeFormatInfo.InvariantInfo);
        IgnoredArticles = string.Empty;
        Artists = new Collection<Artist>(artistList.Select(artist => new Artist(artist)).ToList());
        ChildCollection = new Collection<Child>(songList.Select(song => new Child(song, parentIsArtist: true)).ToList());

        var indexList = new List<Index>();
        foreach (var artist in artistList)
        {
            string indexName = GetIndexKey(artist);
            if (!indexList.Exists(idx => idx.Name == indexName))
            {
                indexList.Add(new Index(indexName));
            }

            int aIdx = indexList.FindIndex(idx => idx.Name == indexName);
            indexList[aIdx].Artists = indexList[aIdx].Artists.Append(new Artist(artist));
        }

        IndexCollection = new Collection<Index>(indexList);
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
    /// Gets artists collection.
    /// </summary>
    [XmlElement("shortcut")]
    [JsonPropertyName("shortcut")]
    public Collection<Artist> Artists { get; }

    /// <summary>
    /// Gets index collection.
    /// </summary>
    [XmlElement("index")]
    [JsonPropertyName("index")]
    public Collection<Index> IndexCollection { get; }

    /// <summary>
    /// Gets child collection.
    /// </summary>
    [XmlElement("child")]
    [JsonPropertyName("child")]
    public Collection<Child> ChildCollection { get; }

    private static string GetIndexKey(BaseItem item)
    {
        return char.IsLetter(item.SortName.First()) ? item.SortName.First().ToString() : "#";
    }
}
