using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using JellySonic.Types;

namespace JellySonic.Models;

/// <summary>
/// A Subsonic artist info data type.
/// </summary>
public class ArtistInfo : ArtistInfoBase, IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistInfo"/> class.
    /// </summary>
    public ArtistInfo()
    {
        SimilarArtists = new Collection<Artist>();
    }

    /// <summary>
    /// Gets a collection of similar artists.
    /// </summary>
    [XmlElement("similarArtist")]
    [JsonPropertyName("similarArtist")]
    public Collection<Artist> SimilarArtists { get; }
}
