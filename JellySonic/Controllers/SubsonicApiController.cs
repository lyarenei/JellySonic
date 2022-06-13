using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using Jellyfin.Data.Entities;
using JellySonic.Models;
using JellySonic.Services;
using JellySonic.Types;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JellySonic.Controllers;

/// <summary>
/// The controller for JellySonic API.
/// </summary>
[ApiController]
[Route("subsonic/rest")]
public class SubsonicApiController : ControllerBase
{
    private readonly ILogger<SubsonicApiController> _logger;
    private readonly JellyfinHelper _jellyfinHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubsonicApiController"/> class.
    /// </summary>
    /// <param name="loggerFactory"> Logger factory.</param>
    /// <param name="userManager">User manager instance.</param>
    /// <param name="libraryManager">Library manager instance.</param>
    public SubsonicApiController(ILoggerFactory loggerFactory, IUserManager userManager, ILibraryManager libraryManager)
    {
        _logger = loggerFactory.CreateLogger<SubsonicApiController>();
        _jellyfinHelper = new JellyfinHelper(loggerFactory, userManager, libraryManager);
    }

    private delegate ActionResult SubsonicAction(User user, SubsonicParams subsonicParams);

    /// <summary>
    /// API Middleware workaround.
    /// Catches all requests and then calls methods according to the path.
    /// </summary>
    /// <param name="action">Controller action name.</param>
    /// <returns>Data response. Varies with endpoint.</returns>
    [HttpGet]
    [HttpPost]
    [Route("{*action}")]
    public ActionResult SubsonicApiMiddleware(string action)
    {
        SubsonicError? err;
        var requestParams = GetRequestParams();
        if (requestParams.RequiredParamsMissing())
        {
            err = new SubsonicError("required common parameter (username, client, ...) missing", ErrorCodes.MissingParam);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var user = AuthenticateUser(requestParams);
        if (user == null)
        {
            err = new SubsonicError("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        string missingParam;
        SubsonicAction subsonicAction;
        string methodName = GetMethodName();
        switch (methodName)
        {
            case "getAlbum":
                missingParam = requestParams.RequiredParamsMissing("id");
                subsonicAction = GetAlbum;
                break;
            case "getArtist":
                missingParam = requestParams.RequiredParamsMissing("id");
                subsonicAction = GetArtist;
                break;
            case "getArtists":
                missingParam = string.Empty;
                subsonicAction = GetArtists;
                break;
            case "ping":
                missingParam = string.Empty;
                subsonicAction = Ping;
                break;
            case "getLicense":
                missingParam = string.Empty;
                subsonicAction = GetLicense;
                break;
            case "getSong":
                missingParam = requestParams.RequiredParamsMissing("id");
                subsonicAction = GetSong;
                break;
            case "getMusicFolders":
                missingParam = string.Empty;
                subsonicAction = GetMusicFolders;
                break;
            case "getMusicDirectory":
                missingParam = requestParams.RequiredParamsMissing("id");
                subsonicAction = GetMusicDirectory;
                break;
            case "getCoverArt":
                missingParam = requestParams.RequiredParamsMissing("id");
                subsonicAction = GetCoverArt;
                break;
            case "download":
            case "stream":
                missingParam = requestParams.RequiredParamsMissing("id");
                subsonicAction = Download;
                break;
            case "getGenres":
                missingParam = string.Empty;
                subsonicAction = GetGenres;
                break;
            case "getIndexes":
                missingParam = string.Empty;
                subsonicAction = GetIndexes;
                break;
            case "getAlbumList":
            case "getAlbumList2":
                missingParam = requestParams.RequiredParamsMissing("type");
                subsonicAction = GetAlbumList;
                break;
            case "search2":
            case "search3":
                missingParam = requestParams.RequiredParamsMissing("query");
                subsonicAction = Search;
                break;
            case "getUser":
                missingParam = requestParams.RequiredParamsMissing("username");
                subsonicAction = GetSubsonicUser;
                break;
            case "getArtistInfo":
            case "getArtistInfo2":
                missingParam = requestParams.RequiredParamsMissing("id");
                subsonicAction = GetArtistInfo;
                break;
            default:
                _logger.LogDebug("method {MethodName} not found", methodName);
                missingParam = string.Empty;
                subsonicAction = (_, _) => NotFound();
                break;
        }

        if (string.IsNullOrEmpty(missingParam))
        {
            return subsonicAction(user, requestParams);
        }

        err = new SubsonicError($"required parameter missing: {missingParam}", ErrorCodes.MissingParam);
        return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
    }

    /// <summary>
    /// Authenticate user in Jellyfin.
    /// </summary>
    /// <param name="requestParams">Request parameters.</param>
    /// <returns>User instance if authentication is successful. Null otherwise.</returns>
    private User? AuthenticateUser(SubsonicParams requestParams)
    {
        var user = _jellyfinHelper.GetUserByUsername(requestParams.Username);

        var jsUser = JellySonic.Instance?.Configuration.Users
            .FirstOrDefault(u => u.JellyfinUserId == user.Id);

        if (jsUser == null)
        {
            _logger.LogError("Failed to load user configuration");
            return null;
        }

        if (string.IsNullOrEmpty(jsUser.Password))
        {
            _logger.LogWarning(
                "Password is not set for user {Username}, " +
                "please set a password in plugin settings to be able to authenticate",
                requestParams.Username);
            return null;
        }

        if (requestParams.TokenAuthPossible())
        {
            var computedToken = Utils.Utils.Md5Hash(jsUser.Password + requestParams.Salt)
                .ToLower(CultureInfo.InvariantCulture);
            return computedToken == requestParams.Token ? user : null;
        }

        if (!string.IsNullOrEmpty(requestParams.Password))
        {
            var password = requestParams.Password;
            if (password.Contains("enc:", StringComparison.InvariantCulture))
            {
                password = Utils.Utils.HexToAscii(password[4..]);
            }

            return password == jsUser.Password ? user : null;
        }

        _logger.LogDebug("Cannot authenticate user - token/salt or password missing");
        return null;
    }

    /// <summary>
    /// Builds output file usable for controller response.
    /// </summary>
    /// <param name="subsonicResponse">Data used to build the response.</param>
    /// <returns>Output file.</returns>
    private FileStreamResult BuildOutput(SubsonicResponse subsonicResponse)
    {
        var format = SubsonicResponse.FormatFromString(GetRequestParams().Format);
        var memoryStream = subsonicResponse.ToMemoryStream(format);
        return format switch
        {
            SubsonicResponse.MemoryStreamFormat.Json => File(memoryStream, "application/json; charset=utf-8"),
            _ => File(memoryStream, "application/xml; charset=utf-8")
        };
    }

    /// <summary>
    /// Get an album.
    /// </summary>
    /// <param name="user">User performing the action.</param>
    /// <param name="subsonicParams">Request parameters.</param>
    /// <returns>Found album or error.</returns>
    private ActionResult GetAlbum(User user, SubsonicParams subsonicParams)
    {
        var album = _jellyfinHelper.GetAlbumById(subsonicParams.Id);
        if (album == null)
        {
            var err = new SubsonicError("album not found", ErrorCodes.DataNotFound);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var albumResponseData = new AlbumWithSongsId3(album);
        return BuildOutput(new SubsonicResponse { ResponseData = albumResponseData });
    }

    /// <summary>
    /// Get an artist.
    /// </summary>
    /// <param name="user">User performing the action.</param>
    /// <param name="subsonicParams">Request parameters.</param>
    /// <returns>Found artist or error.</returns>
    private ActionResult GetArtist(User user, SubsonicParams subsonicParams)
    {
        var artist = _jellyfinHelper.GetArtistById(subsonicParams.Id);
        if (artist == null)
        {
            var err = new SubsonicError("artist not found", ErrorCodes.DataNotFound);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var artistsResponseData = new ArtistWithAlbumsId3(artist);
        return BuildOutput(new SubsonicResponse { ResponseData = artistsResponseData });
    }

    /// <summary>
    /// Get all artists.
    /// </summary>
    /// <param name="user">User performing the action.</param>
    /// <param name="subsonicParams">Request parameters.</param>
    /// <returns>Artists Subsonic response.</returns>
    private ActionResult GetArtists(User user, SubsonicParams subsonicParams)
    {
        string? musicFolderId = subsonicParams.MusicFolderId;
        if (string.IsNullOrEmpty(musicFolderId))
        {
            musicFolderId = null;
        }

        var artists = _jellyfinHelper.GetArtists(user, musicFolderId);
        if (artists == null)
        {
            var err = new SubsonicError("error when retrieving artists", ErrorCodes.Generic);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var artistsResponseData = new ArtistsId3(artists);
        return BuildOutput(new SubsonicResponse { ResponseData = artistsResponseData });
    }

    /// <summary>
    /// Used to test connectivity with the server.
    /// </summary>
    /// <param name="user">User performing the action.</param>
    /// <param name="subsonicParams">Request parameters.</param>
    /// <returns>Empty Subsonic response.</returns>
    private ActionResult Ping(User user, SubsonicParams subsonicParams)
    {
        _logger.LogDebug("received ping request");
        return BuildOutput(new SubsonicResponse());
    }

    /// <summary>
    /// Get details about the Subsonic software license.
    /// </summary>
    /// <param name="user">User performing the action.</param>
    /// <param name="subsonicParams">Request parameters.</param>
    /// <returns>A Subsonic <see cref="License"/> response.</returns>
    private ActionResult GetLicense(User user, SubsonicParams subsonicParams)
    {
        _logger.LogDebug("received getLicense request");
        return BuildOutput(new SubsonicResponse { ResponseData = new License() });
    }

    /// <summary>
    /// Get a song.
    /// </summary>
    /// <param name="user">User performing the action.</param>
    /// <param name="subsonicParams">Request parameters.</param>
    /// <returns>A subsonic <see cref="Child"/> response.</returns>
    private ActionResult GetSong(User user, SubsonicParams subsonicParams)
    {
        var song = _jellyfinHelper.GetSongById(subsonicParams.Id);
        if (song == null)
        {
            var err = new SubsonicError("song not found", ErrorCodes.DataNotFound);
            return BuildOutput(new SubsonicResponse() { ResponseData = err });
        }

        var songResponseData = new Child(song);
        return BuildOutput(new SubsonicResponse { ResponseData = songResponseData });
    }

    /// <summary>
    /// Get all music folders.
    /// </summary>
    /// <param name="user">User performing the action.</param>
    /// <param name="subsonicParams">Request parameters.</param>
    /// <returns>A Subsonic music folders response.</returns>
    private ActionResult GetMusicFolders(User user, SubsonicParams subsonicParams)
    {
        var folders = _jellyfinHelper.GetFolders(user);
        if (folders == null)
        {
            var err = new SubsonicError("folders not found", ErrorCodes.DataNotFound);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var foldersResponseData = new MusicFolders(folders);
        return BuildOutput(new SubsonicResponse { ResponseData = foldersResponseData });
    }

    /// <summary>
    /// Get all music folders.
    /// </summary>
    /// <param name="user">User performing the action.</param>
    /// <param name="subsonicParams">Request parameters.</param>
    /// <returns>A Subsonic music folders response.</returns>
    private ActionResult GetMusicDirectory(User user, SubsonicParams subsonicParams)
    {
        var directory = _jellyfinHelper.GetDirectoryById(subsonicParams.Id);
        if (directory == null)
        {
            var err = new SubsonicError("directory not found", ErrorCodes.DataNotFound);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var directoryResponseData = new SubsonicDirectory(directory);
        return BuildOutput(new SubsonicResponse { ResponseData = directoryResponseData });
    }

    /// <summary>
    /// Get cover art.
    /// </summary>
    /// <param name="user">User performing the action.</param>
    /// <param name="subsonicParams">Request parameters.</param>
    /// <returns>Cover art as binary data.</returns>
    private ActionResult GetCoverArt(User user, SubsonicParams subsonicParams)
    {
        var item = _jellyfinHelper.GetItemById(subsonicParams.Id);
        if (string.IsNullOrEmpty(item?.PrimaryImagePath))
        {
            return NoContent();
        }

        var fs = new FileStream(item.PrimaryImagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return File(fs, MediaTypeNames.Image.Jpeg);
    }

    /// <summary>
    /// Download an item.
    /// </summary>
    /// <param name="user">User performing the action.</param>
    /// <param name="subsonicParams">Request parameters.</param>
    /// <returns>Item as binary data.</returns>
    private ActionResult Download(User user, SubsonicParams subsonicParams)
    {
        var item = _jellyfinHelper.GetItemById(subsonicParams.Id);
        if (item == null)
        {
            var err = new SubsonicError("item not found", ErrorCodes.DataNotFound);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var fs = new FileStream(item.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return new FileStreamResult(fs, Utils.Utils.GetMimeType(item.Path));
    }

    /// <summary>
    /// Get genres.
    /// </summary>
    /// <param name="user">User performing the action.</param>
    /// <param name="subsonicParams">Request parameters.</param>
    /// <returns>A Subsonic genres response.</returns>
    private ActionResult GetGenres(User user, SubsonicParams subsonicParams)
    {
        var artists = _jellyfinHelper.GetArtists(user);
        if (artists == null)
        {
            var err = new SubsonicError("error when retrieving artists", ErrorCodes.Generic);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var genresResponseData = new Genres(artists);
        return BuildOutput(new SubsonicResponse { ResponseData = genresResponseData });
    }

    /// <summary>
    /// Get indexes.
    /// </summary>
    /// <param name="user">User performing the action.</param>
    /// <param name="subsonicParams">Request parameters.</param>
    /// <returns>A Subsonic indexes response.</returns>
    private ActionResult GetIndexes(User user, SubsonicParams subsonicParams)
    {
        string? musicFolderId = subsonicParams.MusicFolderId;
        if (string.IsNullOrEmpty(musicFolderId))
        {
            musicFolderId = null;
        }

        var artists = _jellyfinHelper.GetArtists(user, musicFolderId);
        if (artists == null)
        {
            var err = new SubsonicError("error when retrieving artists", ErrorCodes.Generic);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        if (!string.IsNullOrEmpty(GetRequestParams().IfModifiedSince))
        {
            var lastModified = artists.Max(artist => artist.DateModified);
            var ifModifiedSince = Convert.ToInt64(GetRequestParams().IfModifiedSince, NumberFormatInfo.InvariantInfo);
            var ifModifiedSinceDt = DateTimeOffset.FromUnixTimeMilliseconds(ifModifiedSince);

            if (ifModifiedSinceDt >= lastModified)
            {
                return BuildOutput(new SubsonicResponse());
            }
        }

        var songs = _jellyfinHelper.GetAllSongs(musicFolderId);

        var indexesResponseData = new Indexes(artists, songs);
        return BuildOutput(new SubsonicResponse { ResponseData = indexesResponseData });
    }

    /// <summary>
    /// Get album list.
    /// </summary>
    /// <param name="user">User performing the action.</param>
    /// <param name="subsonicParams">Request parameters.</param>
    /// <returns>A Subsonic album list or album list 2 response.</returns>
    private ActionResult GetAlbumList(User user, SubsonicParams subsonicParams)
    {
        var (albums, error) = GetAlbumsOrError(user, subsonicParams);
        if (error != null)
        {
            return error;
        }

        if (albums == null)
        {
            var err = new SubsonicError("error when retrieving albums", ErrorCodes.Generic);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        IResponseData albumListResponseData;
        if (Request.Path.ToUriComponent().Contains("getAlbumList2", StringComparison.InvariantCulture))
        {
            albumListResponseData = new AlbumList2(albums);
        }
        else
        {
            albumListResponseData = new AlbumList(albums);
        }

        return BuildOutput(new SubsonicResponse { ResponseData = albumListResponseData });
    }

    /// <summary>
    /// Get items matching a search query.
    /// </summary>
    /// <param name="user">User performing the action.</param>
    /// <param name="subsonicParams">Request parameters.</param>
    /// <returns>A Subsonic search result 2 or 3 response.</returns>
    private ActionResult Search(User user, SubsonicParams subsonicParams)
    {
        var artists = PerformSearch("artists", subsonicParams);
        if (artists == null)
        {
            var err = new SubsonicError("error when retrieving artists", ErrorCodes.Generic);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var albums = PerformSearch("albums", subsonicParams);
        if (albums == null)
        {
            var err = new SubsonicError("error when retrieving albums", ErrorCodes.Generic);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var songs = PerformSearch("songs", subsonicParams);
        if (songs == null)
        {
            var err = new SubsonicError("error when retrieving songs", ErrorCodes.Generic);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        IResponseData searchResponseData;
        if (Request.Path.ToUriComponent().Contains("search2", StringComparison.InvariantCulture))
        {
            searchResponseData = new SearchResult2(artists, albums, songs);
        }
        else
        {
            searchResponseData = new SearchResult3(artists, albums, songs);
        }

        return BuildOutput(new SubsonicResponse { ResponseData = searchResponseData });
    }

    /// <summary>
    /// Get user with specified ID.
    /// </summary>
    /// <param name="user">User performing the action.</param>
    /// <param name="subsonicParams">Request parameters.</param>
    /// <returns>A Subsonic user response.</returns>
    private ActionResult GetSubsonicUser(User user, SubsonicParams subsonicParams)
    {
        var jellyfinUser = _jellyfinHelper.GetUserByUsername(GetRequestParams().Username);
        var userResponseData = new SubsonicUser(jellyfinUser);
        return BuildOutput(new SubsonicResponse { ResponseData = userResponseData });
    }

    /// <summary>
    /// Get artist info.
    /// </summary>
    /// <param name="user">User performing the action.</param>
    /// <param name="subsonicParams">Request parameters.</param>
    /// <returns>A Subsonic user response.</returns>
    private ActionResult GetArtistInfo(User user, SubsonicParams subsonicParams)
    {
        var requestParams = GetRequestParams();
        var artist = _jellyfinHelper.GetArtistById(requestParams.Id);
        if (artist == null)
        {
            var err = new SubsonicError("error when retrieving artist", ErrorCodes.Generic);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        // TODO load data from musicbrainz?
        IResponseData artistInfoResponseData;
        if (Request.Path.ToUriComponent().Contains("getArtistInfo2", StringComparison.InvariantCulture))
        {
            artistInfoResponseData = new ArtistInfo2();
        }
        else
        {
            artistInfoResponseData = new ArtistInfo();
        }

        return BuildOutput(new SubsonicResponse { ResponseData = artistInfoResponseData });
    }

    private (IEnumerable<BaseItem>? Albums, ActionResult? Error) GetAlbumsOrError(User user, SubsonicParams subsonicParams)
    {
        string type = subsonicParams.Type;
        if (!int.TryParse(subsonicParams.Size, out var size))
        {
            _logger.LogWarning("Failed to parse size parameter as a number, will use default value");
            size = 10;
        }

        if (!int.TryParse(subsonicParams.Offset, out var offset))
        {
            _logger.LogWarning("Failed to parse offset as a number, will use default value");
            offset = 0;
        }

        int? fromYear = null;
        int? toYear = null;
        if (subsonicParams.Type == "byYear")
        {
            if (!int.TryParse(subsonicParams.FromYear, out var fYear))
            {
                _logger.LogWarning("Failed to parse fromYear as a number");
                var err = new SubsonicError("invalid fromYear parameter value", ErrorCodes.Generic);
                return (null, BuildOutput(new SubsonicResponse("failed") { ResponseData = err }));
            }

            if (!int.TryParse(subsonicParams.ToYear, out var tYear))
            {
                _logger.LogWarning("Failed to parse toYear as a number");
                var err = new SubsonicError("invalid toYear parameter value", ErrorCodes.Generic);
                return (null, BuildOutput(new SubsonicResponse("failed") { ResponseData = err }));
            }

            fromYear = fYear;
            toYear = tYear;
        }

        string? genre = null;
        if (subsonicParams.Type == "byGenre")
        {
            genre = subsonicParams.Genre;
            if (string.IsNullOrEmpty(genre))
            {
                _logger.LogWarning("Genre parameter must be set if type parameter is set to byGenre");
                var err = new SubsonicError("genre parameter not set", ErrorCodes.MissingParam);
                return (null, BuildOutput(new SubsonicResponse("failed") { ResponseData = err }));
            }
        }

        string? musciFolderId = subsonicParams.MusicFolderId;
        if (string.IsNullOrEmpty(musciFolderId))
        {
            musciFolderId = null;
        }

        var albums = _jellyfinHelper.GetAlbums(user, type, size, offset, fromYear, toYear, genre, musciFolderId);
        return (albums, null);
    }

    private IEnumerable<BaseItem>? PerformSearch(string searchType, SubsonicParams subsonicParams)
    {
        if (searchType == "artists")
        {
            if (!int.TryParse(subsonicParams.ArtistCount, out var artistCount))
            {
                _logger.LogWarning("Failed to parse size parameter as a number, will use default value");
                artistCount = 20;
            }

            if (!int.TryParse(subsonicParams.ArtistOffset, out var artistOffset))
            {
                _logger.LogWarning("Failed to parse size parameter as a number, will use default value");
                artistOffset = 0;
            }

            return _jellyfinHelper.Search("artists", subsonicParams.Query, artistCount, artistOffset, subsonicParams.MusicFolderId);
        }

        if (searchType == "albums")
        {
            if (!int.TryParse(subsonicParams.AlbumCount, out var albumCount))
            {
                _logger.LogWarning("Failed to parse size parameter as a number, will use default value");
                albumCount = 20;
            }

            if (!int.TryParse(subsonicParams.AlbumOffset, out var albumOffset))
            {
                _logger.LogWarning("Failed to parse size parameter as a number, will use default value");
                albumOffset = 0;
            }

            return _jellyfinHelper.Search("albums", subsonicParams.Query, albumCount, albumOffset, subsonicParams.MusicFolderId);
        }

        if (searchType == "songs")
        {
            if (!int.TryParse(subsonicParams.SongCount, out var songCount))
            {
                _logger.LogWarning("Failed to parse size parameter as a number, will use default value");
                songCount = 10;
            }

            if (!int.TryParse(subsonicParams.SongOffset, out var songOffset))
            {
                _logger.LogWarning("Failed to parse size parameter as a number, will use default value");
                songOffset = 0;
            }

            return _jellyfinHelper.Search("songs", subsonicParams.Query, songCount, songOffset, subsonicParams.MusicFolderId);
        }

        _logger.LogDebug("Invalid search type specified");
        return null;
    }

    private SubsonicParams GetRequestParams()
    {
        return Request.Method == "GET" ? new SubsonicParams(Request.Query) : new SubsonicParams(Request.Form);
    }

    private string GetMethodName()
    {
        var method = Request.Path.ToString().Split("/").LastOrDefault(string.Empty);
        if (string.IsNullOrEmpty(method))
        {
            return string.Empty;
        }

        if (method.Contains('.', StringComparison.InvariantCulture))
        {
            return method.Split('.').First();
        }

        return method;
    }
}
