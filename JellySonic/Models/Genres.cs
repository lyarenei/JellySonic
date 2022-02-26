using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using JellySonic.Types;
using MediaBrowser.Controller.Entities.Audio;

namespace JellySonic.Models;

/// <summary>
/// Subsonic Genres data type.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "XML serialization")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "XML serialization")]
public class Genres : IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Genres"/> class.
    /// </summary>
    public Genres()
    {
        GenreList = new List<Genre>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Genres"/> class.
    /// </summary>
    /// <param name="artists">Artists collection.</param>
    public Genres(IEnumerable<MusicArtist> artists)
    {
        Dictionary<string, int> albumGenres = new Dictionary<string, int>();
        Dictionary<string, int> songGenres = new Dictionary<string, int>();

        foreach (var artist in artists)
        {
            var albums = artist.Children.Cast<MusicAlbum>().ToList();
            foreach (var album in albums)
            {
                foreach (var albumGenre in album.Genres)
                {
                    if (!albumGenres.ContainsKey(albumGenre))
                    {
                        albumGenres.Add(albumGenre, 0);
                    }

                    albumGenres[albumGenre] += 1;
                }

                var songs = album.Tracks.ToList();
                foreach (var songGenre in songs.SelectMany(song => song.Genres))
                {
                    if (!songGenres.ContainsKey(songGenre))
                    {
                        songGenres.Add(songGenre, 0);
                    }

                    songGenres[songGenre] += 1;
                }
            }
        }

        GenreList = BuildGenreCollection(albumGenres, songGenres);
    }

    /// <summary>
    /// Gets or sets collection of genres.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public IEnumerable<Genre> GenreList { get; set; }

    /// <summary>
    /// Gets or sets collection of genres for serialization.
    /// </summary>
    [XmlElement("genre")]
    [JsonPropertyName("genre")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<Genre> GenreListSerialize
    {
        get { return GenreList.ToList(); }
        set { GenreList = value; }
    }

    private static IEnumerable<Genre> BuildGenreCollection(
        Dictionary<string, int> albumGenres,
        Dictionary<string, int> songGenres)
    {
        var genres = new List<Genre>();
        foreach (var (name, count) in albumGenres)
        {
            if (!genres.Exists(genre => genre.Name == name))
            {
                genres.Add(new Genre(name, 0, 0));
            }

            var idx = genres.FindIndex(genre => genre.Name == name);
            genres[idx].IncrementAlbumCount(count);
        }

        foreach (var (name, count) in songGenres)
        {
            if (!genres.Exists(genre => genre.Name == name))
            {
                genres.Add(new Genre(name, 0, 0));
            }

            var idx = genres.FindIndex(genre => genre.Name == name);
            genres[idx].IncrementSongCount(count);
        }

        return genres;
    }
}
