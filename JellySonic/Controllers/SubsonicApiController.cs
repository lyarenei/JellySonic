using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Data.Entities;
using JellySonic.Models;
using JellySonic.Services;
using JellySonic.Types;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using Microsoft.AspNetCore.Http;
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

    /// <summary>
    /// Authenticate user in Jellyfin.
    /// </summary>
    /// <returns>User instance if authentication is successful. Null otherwise.</returns>
    public User? AuthenticateUser()
    {
        var requestParams = GetRequestParams();
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
    public FileStreamResult BuildOutput(SubsonicResponse subsonicResponse)
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
    /// <returns>Found album or error.</returns>
    [HttpGet]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("getAlbum")]
    [Route("getAlbum.view")]
    public ActionResult GetAlbum()
    {
        var user = AuthenticateUser();
        if (user == null)
        {
            var err = new SubsonicError("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var album = _jellyfinHelper.GetAlbumById(GetRequestParams().Id);
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
    /// <returns>Found artist or error.</returns>
    [HttpGet]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("getArtist")]
    [Route("getArtist.view")]
    public ActionResult GetArtist()
    {
        var user = AuthenticateUser();
        if (user == null)
        {
            var err = new SubsonicError("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var artist = _jellyfinHelper.GetArtistById(GetRequestParams().Id);
        if (artist == null)
        {
            var err = new SubsonicError("artist not found", ErrorCodes.DataNotFound);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var albums = _jellyfinHelper.GetAlbumsByArtistId(user, artist.Id);
        if (albums == null)
        {
            var err = new SubsonicError("error when searching albums", ErrorCodes.Generic);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var artistsResponseData = new ArtistWithAlbumsId3(artist, albums)
        {
            AlbumCount = albums.Count()
        };
        return BuildOutput(new SubsonicResponse { ResponseData = artistsResponseData });
    }

    /// <summary>
    /// Get all artists.
    /// </summary>
    /// <returns>Artists Subsonic response.</returns>
    [HttpGet]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("getArtists")]
    [Route("getArtists.view")]
    public ActionResult GetArtists()
    {
        var user = AuthenticateUser();
        if (user == null)
        {
            var err = new SubsonicError("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        string? musicFolderId = GetRequestParams().MusicFolderId;
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
    /// <returns>Empty Subsonic response.</returns>
    [HttpGet]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("ping")]
    [Route("ping.view")]
    public ActionResult Ping()
    {
        _logger.LogDebug("received ping request");
        return BuildOutput(new SubsonicResponse());
    }

    /// <summary>
    /// Get details about the Subsonic software license.
    /// </summary>
    /// <returns>A Subsonic <see cref="License"/> response.</returns>
    [HttpGet]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("getLicense")]
    [Route("getLicense.view")]
    public ActionResult GetLicense()
    {
        _logger.LogDebug("received getLicense request");

        SubsonicResponse resp;
        if (AuthenticateUser() != null)
        {
            resp = new SubsonicResponse { ResponseData = new License() };
        }
        else
        {
            var err = new SubsonicError("invalid credentials", ErrorCodes.InvalidCredentials);
            resp = new SubsonicResponse("failed") { ResponseData = err };
        }

        return BuildOutput(resp);
    }

    /// <summary>
    /// Get a song.
    /// </summary>
    /// <returns>A subsonic <see cref="Child"/> response.</returns>
    [HttpGet]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("getSong")]
    [Route("getSong.view")]
    public ActionResult GetSong()
    {
        var user = AuthenticateUser();
        if (user == null)
        {
            var err = new SubsonicError("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse { ResponseData = err });
        }

        var song = _jellyfinHelper.GetSongById(GetRequestParams().Id);
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
    /// <returns>A Subsonic music folders response.</returns>
    [HttpGet]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("getMusicFolders")]
    [Route("getMusicFolders.view")]
    public ActionResult GetMusicFolders()
    {
        var user = AuthenticateUser();
        if (user == null)
        {
            var err = new SubsonicError("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

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
    /// <returns>A Subsonic music folders response.</returns>
    [HttpGet]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("getMusicDirectory")]
    [Route("getMusicDirectory.view")]
    public ActionResult GetMusicDirectory()
    {
        var user = AuthenticateUser();
        if (user == null)
        {
            var err = new SubsonicError("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var directory = _jellyfinHelper.GetDirectoryById(GetRequestParams().Id);
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
    /// <returns>Cover art as binary data.</returns>
    [HttpGet]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Route("getCoverArt")]
    [Route("getCoverArt.view")]
    public ActionResult GetCoverArt()
    {
        var user = AuthenticateUser();
        if (user == null)
        {
            var err = new SubsonicError("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var item = _jellyfinHelper.GetItemById(GetRequestParams().Id);
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
    /// <returns>Item as binary data.</returns>
    [HttpGet]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("download")]
    [Route("download.view")]
    [Route("stream")]
    [Route("stream.view")]
    public async Task<ActionResult> Download()
    {
        var user = AuthenticateUser();
        if (user == null)
        {
            var err = new SubsonicError("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var item = _jellyfinHelper.GetItemById(GetRequestParams().Id);
        if (item == null)
        {
            var err = new SubsonicError("item not found", ErrorCodes.DataNotFound);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var fs = new FileStream(item.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        byte[] content = new byte[fs.Length];
        await fs.ReadAsync(content, CancellationToken.None).ConfigureAwait(false);
        return new FileContentResult(content, Utils.Utils.GetMimeType(item.Path));
    }

    /// <summary>
    /// Get genres.
    /// </summary>
    /// <returns>A Subsonic genres response.</returns>
    [HttpGet]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("getGenres")]
    [Route("getGenres.view")]
    public ActionResult GetGenres()
    {
        var user = AuthenticateUser();
        if (user == null)
        {
            var err = new SubsonicError("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

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
    /// <returns>A Subsonic indexes response.</returns>
    [HttpGet]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("getIndexes")]
    [Route("getIndexes.view")]
    public ActionResult GetIndexes()
    {
        var user = AuthenticateUser();
        if (user == null)
        {
            var err = new SubsonicError("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        string? musicFolderId = GetRequestParams().MusicFolderId;
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
    /// <returns>A Subsonic album list or album list 2 response.</returns>
    [HttpGet]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("getAlbumList")]
    [Route("getAlbumList.view")]
    [Route("getAlbumList2")]
    [Route("getAlbumList2.view")]
    public ActionResult GetAlbumList()
    {
        var user = AuthenticateUser();
        if (user == null)
        {
            var err = new SubsonicError("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var (albums, error) = GetAlbumsOrError();
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
    /// <returns>A Subsonic search result 2 or 3 response.</returns>
    [HttpGet]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("search2")]
    [Route("search2.view")]
    [Route("search3")]
    [Route("search3.view")]
    public ActionResult Search()
    {
        var user = AuthenticateUser();
        if (user == null)
        {
            var err = new SubsonicError("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var artists = PerformSearch("artists");
        if (artists == null)
        {
            var err = new SubsonicError("error when retrieving artists", ErrorCodes.Generic);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var albums = PerformSearch("albums");
        if (albums == null)
        {
            var err = new SubsonicError("error when retrieving albums", ErrorCodes.Generic);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var songs = PerformSearch("songs");
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
    /// <returns>A Subsonic user response.</returns>
    [HttpGet]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("getUser")]
    [Route("getUser.view")]
    public ActionResult GetSubsonicUser()
    {
        var user = AuthenticateUser();
        if (user == null)
        {
            var err = new SubsonicError("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var jellyfinUser = _jellyfinHelper.GetUserByUsername(GetRequestParams().Username);
        var userResponseData = new SubsonicUser(jellyfinUser);
        return BuildOutput(new SubsonicResponse { ResponseData = userResponseData });
    }

    /// <summary>
    /// Get artist info.
    /// </summary>
    /// <returns>A Subsonic user response.</returns>
    [HttpGet]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("getArtistInfo")]
    [Route("getArtistInfo.view")]
    [Route("getArtistInfo2")]
    [Route("getArtistInfo2.view")]
    public ActionResult GetArtistInfo()
    {
        var user = AuthenticateUser();
        if (user == null)
        {
            var err = new SubsonicError("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var requestParams = GetRequestParams();
        var artist = _jellyfinHelper.GetArtistById(requestParams.Id);
        if (artist == null)
        {
            var err = new SubsonicError("error when retrieving artist", ErrorCodes.Generic);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

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

    private (IEnumerable<BaseItem>? Albums, ActionResult? Error) GetAlbumsOrError()
    {
        var requestParams = GetRequestParams();
        var user = AuthenticateUser();
        if (user == null)
        {
            var err = new SubsonicError("invalid credentials", ErrorCodes.InvalidCredentials);
            return (null, BuildOutput(new SubsonicResponse("failed") { ResponseData = err }));
        }

        string type = requestParams.Type;
        if (!int.TryParse(requestParams.Size, out var size))
        {
            _logger.LogWarning("Failed to parse size parameter as a number, will use default value");
            size = 10;
        }

        if (!int.TryParse(requestParams.Offset, out var offset))
        {
            _logger.LogWarning("Failed to parse offset as a number, will use default value");
            offset = 0;
        }

        int? fromYear = null;
        int? toYear = null;
        if (requestParams.Type == "byYear")
        {
            if (!int.TryParse(requestParams.FromYear, out var fYear))
            {
                _logger.LogWarning("Failed to parse fromYear as a number");
                var err = new SubsonicError("invalid fromYear parameter value", ErrorCodes.Generic);
                return (null, BuildOutput(new SubsonicResponse("failed") { ResponseData = err }));
            }

            if (!int.TryParse(requestParams.ToYear, out var tYear))
            {
                _logger.LogWarning("Failed to parse toYear as a number");
                var err = new SubsonicError("invalid toYear parameter value", ErrorCodes.Generic);
                return (null, BuildOutput(new SubsonicResponse("failed") { ResponseData = err }));
            }

            fromYear = fYear;
            toYear = tYear;
        }

        string? genre = null;
        if (requestParams.Type == "byGenre")
        {
            genre = requestParams.Genre;
            if (string.IsNullOrEmpty(genre))
            {
                _logger.LogWarning("Genre parameter must be set if type parameter is set to byGenre");
                var err = new SubsonicError("genre parameter not set", ErrorCodes.MissingParam);
                return (null, BuildOutput(new SubsonicResponse("failed") { ResponseData = err }));
            }
        }

        var albums = _jellyfinHelper.GetAlbums(user, type, size, offset, fromYear, toYear, genre);
        return (albums, null);
    }

    private IEnumerable<BaseItem>? PerformSearch(string searchType)
    {
        var requestParams = GetRequestParams();
        if (searchType == "artists")
        {
            if (!int.TryParse(requestParams.ArtistCount, out var artistCount))
            {
                _logger.LogWarning("Failed to parse size parameter as a number, will use default value");
                artistCount = 20;
            }

            if (!int.TryParse(requestParams.ArtistOffset, out var artistOffset))
            {
                _logger.LogWarning("Failed to parse size parameter as a number, will use default value");
                artistOffset = 0;
            }

            return _jellyfinHelper.Search("artists", requestParams.Query, artistCount, artistOffset);
        }

        if (searchType == "albums")
        {
            if (!int.TryParse(requestParams.AlbumCount, out var albumCount))
            {
                _logger.LogWarning("Failed to parse size parameter as a number, will use default value");
                albumCount = 20;
            }

            if (!int.TryParse(requestParams.AlbumOffset, out var albumOffset))
            {
                _logger.LogWarning("Failed to parse size parameter as a number, will use default value");
                albumOffset = 0;
            }

            return _jellyfinHelper.Search("albums", requestParams.Query, albumCount, albumOffset);
        }

        if (searchType == "songs")
        {
            if (!int.TryParse(requestParams.SongCount, out var songCount))
            {
                _logger.LogWarning("Failed to parse size parameter as a number, will use default value");
                songCount = 10;
            }

            if (!int.TryParse(requestParams.SongOffset, out var songOffset))
            {
                _logger.LogWarning("Failed to parse size parameter as a number, will use default value");
                songOffset = 0;
            }

            return _jellyfinHelper.Search("songs", requestParams.Query, songCount, songOffset);
        }

        _logger.LogDebug("Invalid search type specified");
        return null;
    }

    private SubsonicParams GetRequestParams()
    {
        return Request.Method == "GET" ? new SubsonicParams(Request.Query) : new SubsonicParams(Request.Form);
    }
}

/// <summary>
/// Subsonic request parameters.
/// </summary>
public class SubsonicParams
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubsonicParams"/> class.
    /// </summary>
    /// <param name="data">Query data.</param>
    public SubsonicParams(IQueryCollection data)
    {
        // TODO This would be very nice to dedupe

        Username = data["u"];
        Password = data["p"];
        Token = data["t"];
        Salt = data["s"];
        Version = data["v"];
        Client = data["c"];
        Format = data["f"];

        Id = data["id"];
        MusicFolderId = data["musicFolderId"];
        IfModifiedSince = data["ifModifiedSince"];
        Count = data["count"];
        IncludeNotPresent = data["includeNotPresent"];
        Artist = data["artist"];
        Type = data["type"];
        Size = data["size"];
        Offset = data["offset"];
        FromYear = data["fromYear"];
        ToYear = data["toYear"];
        Genre = data["genre"];
        Query = data["query"];
        ArtistCount = data["artistCount"];
        ArtistOffset = data["artistOffset"];
        AlbumCount = data["albumCount"];
        AlbumOffset = data["albumOffset"];
        SongCount = data["songCount"];
        SongOffset = data["songOffset"];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubsonicParams"/> class.
    /// </summary>
    /// <param name="data">Form data.</param>
    public SubsonicParams(IFormCollection data)
    {
        Username = data["u"];
        Password = data["p"];
        Token = data["t"];
        Salt = data["s"];
        Version = data["v"];
        Client = data["c"];
        Format = data["f"];

        Id = data["id"];
        MusicFolderId = data["musicFolderId"];
        IfModifiedSince = data["ifModifiedSince"];
        Count = data["count"];
        IncludeNotPresent = data["includeNotPresent"];
        Artist = data["artist"];
        Type = data["type"];
        Size = data["size"];
        Offset = data["offset"];
        FromYear = data["fromYear"];
        ToYear = data["toYear"];
        Genre = data["genre"];
        Query = data["query"];
        ArtistCount = data["artistCount"];
        ArtistOffset = data["artistOffset"];
        AlbumCount = data["albumCount"];
        AlbumOffset = data["albumOffset"];
        SongCount = data["songCount"];
        SongOffset = data["songOffset"];
    }

    /// <summary>
    /// Determines if there's data necessary to perform token authentication.
    /// </summary>
    /// <returns>Token authentication is possible.</returns>
    public bool TokenAuthPossible()
    {
        return !string.IsNullOrEmpty(Token) && !string.IsNullOrEmpty(Salt);
    }

#pragma warning disable CS1591
#pragma warning disable SA1201

    public string SongOffset { get; }

    public string SongCount { get; }

    public string AlbumOffset { get; }

    public string AlbumCount { get; }

    public string ArtistOffset { get; }

    public string ArtistCount { get; }

    public string Query { get; }

    public string Genre { get; }

    public string ToYear { get; }

    public string FromYear { get; }

    public string Offset { get; }

    public string Size { get; }

    public string Type { get; }

    public string IncludeNotPresent { get; }

    public string Count { get; }

    public string IfModifiedSince { get; }

    public string MusicFolderId { get; }

    public string Id { get; }

    public string Artist { get; }

    public string Version { get; }

    public string Salt { get; }

    public string Token { get; }

    public string Username { get; }

    public string Password { get; }

    public string Client { get; }

    public string Format { get; }

#pragma warning restore SA1201
#pragma warning restore CS1591
}
