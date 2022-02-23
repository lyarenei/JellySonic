using System;
using System.Collections.Generic;
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
                    true)
                .ConfigureAwait(false);

            return user.GetAwaiter().GetResult();
        }
        catch (AuthenticationException)
        {
            return null;
        }
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
        var query = new InternalItemsQuery
        {
            IncludeItemTypes = new[] { BaseItemKind.MusicAlbum },
            AlbumArtistIds = new[] { artistId },
            OrderBy = new (string, SortOrder)[] { (ItemSortBy.SortName, SortOrder.Ascending) },
            Recursive = true
        };

        query.SetUser(user);
        var queryData = _libraryManager.GetItemList(query);
        return queryData?.Cast<MusicAlbum>().ToList();
    }

    /// <summary>
    /// Get all artists.
    /// </summary>
    /// <param name="user">User performing the query.</param>
    /// <returns>List of artists. Null if error.</returns>
    public IEnumerable<MusicArtist>? GetArtists(User user)
    {
        var query = new InternalItemsQuery
        {
            OrderBy = new (string, SortOrder)[] { (ItemSortBy.SortName, SortOrder.Ascending) },
            Recursive = true
        };

        query.SetUser(user);
        var queryData = _libraryManager.GetAlbumArtists(query);
        if (queryData == null)
        {
            return null;
        }

        var artists = new List<MusicArtist>();
        foreach (var (item, _) in queryData.Items)
        {
            artists.Add((MusicArtist)item);
        }

        return artists;
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
    /// Get folders.
    /// </summary>
    /// <param name="user">User performing the query.</param>
    /// <returns>List of folders. Null if error.</returns>
    public IEnumerable<Folder>? GetFolders(User user)
    {
        var query = new InternalItemsQuery
        {
            IncludeItemTypes = new[] { BaseItemKind.Folder },
            SourceTypes = new[] { SourceType.Library },
            OrderBy = new (string, SortOrder)[] { (ItemSortBy.SortName, SortOrder.Ascending) },
            Recursive = true
        };

        query.SetUser(user);
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
}
