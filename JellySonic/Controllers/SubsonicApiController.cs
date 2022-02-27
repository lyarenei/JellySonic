using System;
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
using MediaBrowser.Common.Extensions;
using MediaBrowser.Controller.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Directory = JellySonic.Models.Directory;

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
    /// <returns>User authentication is successful.</returns>
    public User? AuthenticateUser()
    {
        string username = Request.Query["u"];
        string password = Request.Query["p"];
        string endpoint = HttpContext.GetNormalizedRemoteIp().ToString();

        if (password.Contains("enc:", StringComparison.InvariantCulture))
        {
            password = Utils.Utils.HexToAscii(password[4..]);
        }

        return _jellyfinHelper.AuthenticateUser(username, password, endpoint);
    }

    /// <summary>
    /// Builds output file usable for controller response.
    /// </summary>
    /// <param name="subsonicResponse">Data used to build the response.</param>
    /// <returns>Output file.</returns>
    public FileStreamResult BuildOutput(SubsonicResponse subsonicResponse)
    {
        var format = SubsonicResponse.FormatFromString(Request.Query["f"]);
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

        var album = _jellyfinHelper.GetAlbumById(Request.Query["id"]);
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

        var artist = _jellyfinHelper.GetArtistById(Request.Query["id"]);
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
            AlbumCount = albums.Count().ToString(NumberFormatInfo.InvariantInfo)
        };
        return BuildOutput(new SubsonicResponse { ResponseData = artistsResponseData });
    }

    /// <summary>
    /// Get all artists.
    /// </summary>
    /// <returns>Artists Subsonic response.</returns>
    [HttpGet]
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

        var artists = _jellyfinHelper.GetArtists(user);
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

        var song = _jellyfinHelper.GetSongById(Request.Query["id"]);
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

        var directory = _jellyfinHelper.GetDirectoryById(Request.Query["id"]);
        if (directory == null)
        {
            var err = new SubsonicError("directory not found", ErrorCodes.DataNotFound);
            return BuildOutput(new SubsonicResponse("failed") { ResponseData = err });
        }

        var directoryResponseData = new Directory(directory);
        return BuildOutput(new SubsonicResponse { ResponseData = directoryResponseData });
    }

    /// <summary>
    /// Get cover art.
    /// </summary>
    /// <returns>Cover art as binary data.</returns>
    [HttpGet]
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

        var item = _jellyfinHelper.GetItemById(Request.Query["id"]);
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

        var item = _jellyfinHelper.GetItemById(Request.Query["id"]);
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

        // TODO fetch and process real data

        var indexesResponseData = new Indexes();
        return BuildOutput(new SubsonicResponse { ResponseData = indexesResponseData });
    }
}
