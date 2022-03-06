using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using JellySonic.Types;
using MediaBrowser.Controller.Entities;

namespace JellySonic.Models;

/// <summary>
/// Subsonic ArtistID3 data type.
/// </summary>
public class ArtistsId3 : IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistsId3"/> class.
    /// </summary>
    public ArtistsId3()
    {
        IndexId3Collection = new Collection<IndexId3>();
        IgnoredArticles = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistsId3"/> class.
    /// </summary>
    /// <param name="artists">A collection of artists data.</param>
    public ArtistsId3(IEnumerable<BaseItem> artists)
    {
        var artistIndex = new List<IndexId3>();
        foreach (var artist in artists)
        {
            string indexName = char.IsLetter(artist.SortName.First()) ? artist.SortName.First().ToString() : "#";
            if (!artistIndex.Exists(idx => idx.Name == indexName))
            {
                artistIndex.Add(new IndexId3(indexName));
            }

            int aIdx = artistIndex.FindIndex(idx => idx.Name == indexName);
            artistIndex[aIdx].Artists = artistIndex[aIdx].Artists.Append(new ArtistId3(artist));
        }

        IndexId3Collection = new Collection<IndexId3>(artistIndex);
        IgnoredArticles = string.Empty;
    }

    /// <summary>
    /// Gets collection of id3 indexes.
    /// </summary>
    [XmlElement("index")]
    [JsonPropertyName("index")]
    public Collection<IndexId3> IndexId3Collection { get; }

    /// <summary>
    /// Gets or sets a series of ignored articles; separated by space.
    /// </summary>
    [XmlAttribute("ignoredArticles")]
    public string IgnoredArticles { get; set; }
}
