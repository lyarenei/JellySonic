using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Jellyfin.Data.Entities;
using Jellyfin.Data.Enums;
using MediaBrowser.Controller.Authentication;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Querying;
using Microsoft.Extensions.Logging;

namespace JellySonic.Services;

/// <summary>
/// Wrapper for requesting Jellyfin data.
/// </summary>
public class JellyfinHelper
{
    private readonly ILogger<JellyfinHelper> _logger;
    private readonly IUserManager _userManager;
    private readonly ILibraryManager _libraryManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="JellyfinHelper"/> class.
    /// </summary>
    /// <param name="loggerFactory">Logger factory.</param>
    /// <param name="userManager">User manager.</param>
    /// <param name="libraryManager">Library manager.</param>
    public JellyfinHelper(ILoggerFactory loggerFactory, IUserManager userManager, ILibraryManager libraryManager)
    {
        _logger = loggerFactory.CreateLogger<JellyfinHelper>();
        _userManager = userManager;
        _libraryManager = libraryManager;
    }

    /// <summary>
    /// Authenticate user by given credentials.
    /// </summary>
    /// <param name="username">User username.</param>
    /// <param name="password">User password.</param>
    /// <param name="remoteEndpoint">Remote endpoint IP.</param>
    /// <returns><see cref="User"/> if authenticated, null otherwise.</returns>
    public User? AuthenticateUser(string username, string password, string remoteEndpoint)
    {
        try
        {
            var user = this._userManager.AuthenticateUser(
                    username,
                    password,
                    string.Empty,
                    remoteEndpoint,
                    false)
                .ConfigureAwait(false);

            return user.GetAwaiter().GetResult();
        }
        catch (AuthenticationException)
        {
            return null;
        }
    }

    /// <summary>
    /// Get user by their username.
    /// </summary>
    /// <param name="username">Username of the user.</param>
    /// <returns>User.</returns>
    public User GetUserByUsername(string username)
    {
        return _userManager.GetUserByName(username);
    }

    /// <summary>
    /// Get album by specified ID.
    /// </summary>
    /// <param name="albumId">IF of the album.</param>
    /// <returns><see cref="MusicAlbum"/>, null on error.</returns>
    public MusicAlbum? GetAlbumById(string albumId)
    {
        try
        {
            return (MusicAlbum)_libraryManager.GetItemById(albumId);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Get artist by specified ID.
    /// </summary>
    /// <param name="artistId">ID of the artist.</param>
    /// <returns><see cref="MusicArtist"/>.</returns>
    public MusicArtist? GetArtistById(string artistId)
    {
        try
        {
            return (MusicArtist)_libraryManager.GetItemById(artistId);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Get all albums of a specified artist ID.
    /// </summary>
    /// <param name="user">User performing the query.</param>
    /// <param name="artistId">ID of the artist.</param>
    /// <returns>List of found albums. Null if error.</returns>
    public IEnumerable<MusicAlbum>? GetAlbumsByArtistId(User user, Guid artistId)
    {
        var query = new InternalItemsQuery(user)
        {
            IncludeItemTypes = new[] { BaseItemKind.MusicAlbum },
            AlbumArtistIds = new[] { artistId },
            OrderBy = new (string, SortOrder)[] { (ItemSortBy.SortName, SortOrder.Ascending) },
            Recursive = true
        };

        var queryData = _libraryManager.GetItemList(query);
        return queryData?.Cast<MusicAlbum>().ToList();
    }

    /// <summary>
    /// Get all artists.
    /// If musicFolderId parameter is not null, only include artists from this folder.
    /// </summary>
    /// <param name="user">User performing the query.</param>
    /// <param name="musicFolderId">ID folder to operate with.</param>
    /// <returns>Collection of artists. Null if error.</returns>
    public Collection<MusicArtist>? GetArtists(User user, string? musicFolderId = null)
    {
        var query = new InternalItemsQuery(user)
        {
            OrderBy = new (string, SortOrder)[] { (ItemSortBy.SortName, SortOrder.Ascending) },
            Recursive = true,
        };

        if (musicFolderId != null)
        {
            query.TopParentIds = new[] { new Guid(musicFolderId) };
        }

        var queryData = _libraryManager.GetAlbumArtists(query);
        if (queryData == null)
        {
            return null;
        }

        var artists = queryData.Items.Select(tuple => tuple.Item).Cast<MusicArtist>().ToList();
        return new Collection<MusicArtist>(artists);
    }

    /// <summary>
    /// Get a song by specified ID.
    /// </summary>
    /// <param name="songId">ID of the song.</param>
    /// <returns><see cref="Audio"/>. Null if error.</returns>
    public Audio? GetSongById(string songId)
    {
        try
        {
            return (Audio)_libraryManager.GetItemById(songId);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Get all songs.
    /// If musicFolderId parameter is set, only include songs from this folder.
    /// </summary>
    /// <param name="musicFolderId">ID of music folder.</param>
    /// <returns>Collection of all songs in the library.</returns>
    public IEnumerable<BaseItem> GetAllSongs(string? musicFolderId = null)
    {
        var query = new InternalItemsQuery
        {
            IncludeItemTypes = new[] { BaseItemKind.Audio },
            OrderBy = new (string, SortOrder)[] { (ItemSortBy.SortName, SortOrder.Ascending) },
            Recursive = true
        };

        if (!string.IsNullOrEmpty(musicFolderId))
        {
            query.TopParentIds = new[] { new Guid(musicFolderId) };
        }

        return _libraryManager.GetItemList(query);
    }

    /// <summary>
    /// Get folders.
    /// </summary>
    /// <param name="user">User performing the query.</param>
    /// <returns>List of folders. Null if error.</returns>
    public IEnumerable<Folder>? GetFolders(User user)
    {
        var query = new InternalItemsQuery(user)
        {
            IncludeItemTypes = new[] { BaseItemKind.Folder },
            SourceTypes = new[] { SourceType.Library },
            OrderBy = new (string, SortOrder)[] { (ItemSortBy.SortName, SortOrder.Ascending) },
            Recursive = true
        };

        var queryData = _libraryManager.GetItemList(query);
        return queryData?.Cast<Folder>().ToList();
    }

    /// <summary>
    /// Get a directory by specified ID.
    /// </summary>
    /// <param name="dirId">ID of the directory.</param>
    /// <returns><see cref="Folder"/>. Null if error.</returns>
    public Folder? GetDirectoryById(string dirId)
    {
        try
        {
            return (Folder)_libraryManager.GetItemById(dirId);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Get item by specified ID.
    /// </summary>
    /// <param name="itemId">ID of the item.</param>
    /// <returns><see cref="BaseItem"/>. Null if error.</returns>
    public BaseItem? GetItemById(string itemId)
    {
        return _libraryManager.GetItemById(itemId);
    }

    /// <summary>
    /// Get albums with specified query params.
    /// </summary>
    /// <param name="user">User performing the query.</param>
    /// <param name="type">Query type.</param>
    /// <param name="size">Query result size. Default 10.</param>
    /// <param name="offset">Query result offset. Default 0.</param>
    /// <param name="fromYear">Include only albums released in specified year and until year specified in <paramref name="toYear"/>.
    /// Must not be null and has effect only if <paramref name="type"/> is set to byYear.</param>
    /// <param name="toYear">Include only albums released in specified year and earlier until year specified in <paramref name="fromYear"/>.
    /// Must not be null and has effect only if <paramref name="type"/> is set to byYear.</param>
    /// <param name="genre">Include only albums of specified genre. Has effect only if <paramref name="type"/> is set to byGenre.</param>
    /// <returns>A collection of items. Null if error.</returns>
    public IEnumerable<BaseItem>? GetAlbums(
        User user,
        string type,
        int size = 10,
        int offset = 0,
        int? fromYear = null,
        int? toYear = null,
        string? genre = null)
    {
        string sortBy;
        SortOrder sortOrder = SortOrder.Ascending;
        bool? isLikedOrFav = type == "starred" ? true : null;
        int[]? years = null;
        switch (type)
        {
            case "random":
                sortBy = ItemSortBy.Random;
                break;
            case "newest":
                sortBy = ItemSortBy.PremiereDate;
                sortOrder = SortOrder.Descending;
                break;
            case "highest":
                sortBy = ItemSortBy.OfficialRating;
                sortOrder = SortOrder.Descending;
                break;
            case "recent":
                sortBy = ItemSortBy.DateCreated;
                sortOrder = SortOrder.Descending;
                break;
            case "alphabeticalByName":
            case "starred":
            case "byGenre":
                sortBy = ItemSortBy.SortName;
                break;
            case "alphabeticalByArtist":
                sortBy = ItemSortBy.AlbumArtist;
                break;
            case "byYear":
                sortBy = ItemSortBy.ProductionYear;
                sortOrder = fromYear <= toYear ? SortOrder.Ascending : SortOrder.Descending;
                int start = fromYear <= toYear ? fromYear!.Value : toYear!.Value;
                int end = fromYear <= toYear ? toYear!.Value : fromYear!.Value;
                years = Enumerable.Range(start, end - start).ToArray();
                break;
            default:
                _logger.LogWarning("Requested album list of type '{QueryType}', but this type is not valid or implemented", type);
                return new List<BaseItem>();
        }

        return GetAlbumsByParams(user, sortBy, sortOrder, size, offset, isLikedOrFav, years, genre);
    }

    /// <summary>
    /// Performs a search according to specified query.
    /// </summary>
    /// <param name="searchType">Type of search - artists, albums, songs.</param>
    /// <param name="query">Search query.</param>
    /// <param name="count">Search result count. Default 20.</param>
    /// <param name="offset">Search result offset. Default 0.</param>
    /// <returns>A collection of items matching the query.</returns>
    public IEnumerable<BaseItem>? Search(string searchType, string query, int count = 20, int offset = 0)
    {
        BaseItemKind searchItem;
        switch (searchType)
        {
            case "artists":
                searchItem = BaseItemKind.MusicArtist;
                break;
            case "albums":
                searchItem = BaseItemKind.MusicAlbum;
                break;
            case "songs":
                searchItem = BaseItemKind.Audio;
                break;
            default:
                _logger.LogWarning("Invalid search type '{SearchType}'", searchType);
                return new List<BaseItem>();
        }

        return DoSearch(searchItem, query, count, offset);
    }

    private IEnumerable<BaseItem>? GetAlbumsByParams(
        User user,
        string sortBy,
        SortOrder sortOrder = SortOrder.Ascending,
        int size = 10,
        int offset = 0,
        bool? isLikedOrFav = null,
        int[]? years = null,
        string? genre = null)
    {
        var query = new InternalItemsQuery(user)
        {
            IncludeItemTypes = new[] { BaseItemKind.MusicAlbum },
            OrderBy = new (string, SortOrder)[] { (sortBy, sortOrder) },
            Recursive = true,
            Limit = size,
            StartIndex = offset
        };

        if (isLikedOrFav != null)
        {
            query.IsFavoriteOrLiked = isLikedOrFav.Value;
        }
        else if (years != null)
        {
            query.Years = years;
        }
        else if (!string.IsNullOrEmpty(genre))
        {
            query.Genres = new[] { genre };
        }

        var queryData = _libraryManager.GetItemList(query);
        return queryData?.ToList();
    }

    private IEnumerable<BaseItem>? DoSearch(BaseItemKind searchItem, string queryString, int count, int offset)
    {
        var query = new InternalItemsQuery
        {
            NameContains = queryString,
            IncludeItemTypes = new[] { searchItem },
            OrderBy = new (string, SortOrder)[] { (ItemSortBy.SortName, SortOrder.Ascending) },
            Recursive = true,
            Limit = count,
            StartIndex = offset,
            MinSimilarityScore = 20
        };

        return _libraryManager.GetItemList(query);
    }
}
