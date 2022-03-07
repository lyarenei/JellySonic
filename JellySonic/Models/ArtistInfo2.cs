using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using JellySonic.Types;

namespace JellySonic.Models;

/// <summary>
/// A Subsonic artist info 2 data type.
/// </summary>
public class ArtistInfo2 : ArtistInfoBase, IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistInfo2"/> class.
    /// </summary>
    public ArtistInfo2()
    {
        SimilarArtists = new Collection<ArtistId3>();
    }

    /// <summary>
    /// Gets a collection of similar artists.
    /// </summary>
    [XmlElement("similarArtist")]
    [JsonPropertyName("similarArtist")]
    public Collection<ArtistId3> SimilarArtists { get; }
}
