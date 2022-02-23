using System.IO;
using System.Net.Mime;
using Jellyfin.Data.Entities;
using JellySonic.Models;
using JellySonic.Services;
using MediaBrowser.Common.Extensions;
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
    private readonly ILibraryManager _libraryManager;
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
        _libraryManager = libraryManager;
        _jellyfinHelper = new JellyfinHelper(loggerFactory, userManager, libraryManager);
    }

    /// <summary>
    /// Authenticate user in Jellyfin.
    /// </summary>
    /// <returns>User authentication is successful.</returns>
    public User? AuthenticateUser()
    {
        var username = Request.Query["u"];
        var password = Request.Query["p"];
        return _jellyfinHelper.AuthenticateUser(username, password, HttpContext.GetNormalizedRemoteIp().ToString());
    }

    /// <summary>
    /// Builds output file usable for controller response.
    /// </summary>
    /// <param name="subsonicResponse">Data used to build the response.</param>
    /// <returns>Output file.</returns>
    public FileStreamResult BuildOutput(SubsonicResponse subsonicResponse)
    {
        var memoryStream = subsonicResponse.ToMemoryStream();
        return File(memoryStream, "application/xml; charset=utf-8");
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
            var err = new ErrorResponseData("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse { ResponseData = err });
        }

        var album = _jellyfinHelper.GetAlbumById(Request.Query["id"]);
        if (album == null)
        {
            var err = new ErrorResponseData("album not found", ErrorCodes.DataNotFound);
            return BuildOutput(new SubsonicResponse() { ResponseData = err });
        }

        var albumResponseData = new AlbumResponseData(album);
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
            var err = new ErrorResponseData("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse() { ResponseData = err });
        }

        var artist = _jellyfinHelper.GetArtistById(Request.Query["id"]);
        if (artist == null)
        {
            var err = new ErrorResponseData("artist not found", ErrorCodes.DataNotFound);
            return BuildOutput(new SubsonicResponse() { ResponseData = err });
        }

        var albums = _jellyfinHelper.GetAlbumsByArtistId(user, artist.Id);
        if (albums == null)
        {
            var err = new ErrorResponseData("error when searching albums", ErrorCodes.Generic);
            return BuildOutput(new SubsonicResponse() { ResponseData = err });
        }

        var artistsResponseData = new ArtistResponseData(artist, albums);
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
            var err = new ErrorResponseData("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse { ResponseData = err });
        }

        var artists = _jellyfinHelper.GetArtists(user);
        if (artists == null)
        {
            var err = new ErrorResponseData("error when retrieving artists", ErrorCodes.Generic);
            return BuildOutput(new SubsonicResponse { ResponseData = err });
        }

        var artistsResponseData = new ArtistsResponseData(artists);
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
    /// <returns>A Subsonic <see cref="LicenseResponseData"/> response.</returns>
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
            resp = new SubsonicResponse { ResponseData = new LicenseResponseData() };
        }
        else
        {
            var err = new ErrorResponseData("invalid credentials", ErrorCodes.InvalidCredentials);
            resp = new SubsonicResponse("failed") { ResponseData = err };
        }

        return BuildOutput(resp);
    }

    /// <summary>
    /// Get a song.
    /// </summary>
    /// <returns>A subsonic <see cref="Song"/> response.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("getSong")]
    [Route("getSong.view")]
    public ActionResult GetSong()
    {
        var user = AuthenticateUser();
        if (user == null)
        {
            var err = new ErrorResponseData("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse { ResponseData = err });
        }

        var song = _jellyfinHelper.GetSongById(Request.Query["id"]);
        if (song == null)
        {
            var err = new ErrorResponseData("song not found", ErrorCodes.DataNotFound);
            return BuildOutput(new SubsonicResponse() { ResponseData = err });
        }

        var songResponseData = new SongResponseData(song);
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
            var err = new ErrorResponseData("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse { ResponseData = err });
        }

        var folders = _jellyfinHelper.GetFolders(user);
        if (folders == null)
        {
            var err = new ErrorResponseData("folders not found", ErrorCodes.DataNotFound);
            return BuildOutput(new SubsonicResponse() { ResponseData = err });
        }

        var foldersResponseData = new MusicFoldersResponseData(folders);
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
            var err = new ErrorResponseData("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse { ResponseData = err });
        }

        var directory = _jellyfinHelper.GetDirectoryById(Request.Query["id"]);
        if (directory == null)
        {
            var err = new ErrorResponseData("directory not found", ErrorCodes.DataNotFound);
            return BuildOutput(new SubsonicResponse() { ResponseData = err });
        }

        var directoryResponseData = new DirectoryResponseData(directory);
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
            var err = new ErrorResponseData("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse { ResponseData = err });
        }

        var item = _jellyfinHelper.GetItemById(Request.Query["id"]);
        if (item == null)
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
    public ActionResult Download()
    {
        var user = AuthenticateUser();
        if (user == null)
        {
            var err = new ErrorResponseData("invalid credentials", ErrorCodes.InvalidCredentials);
            return BuildOutput(new SubsonicResponse { ResponseData = err });
        }

        var item = _jellyfinHelper.GetItemById(Request.Query["id"]);
        if (item == null)
        {
            var err = new ErrorResponseData("item not found", ErrorCodes.DataNotFound);
            return BuildOutput(new SubsonicResponse { ResponseData = err });
        }

        var fs = new FileStream(item.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return File(fs, MediaTypeNames.Application.Octet);
    }
}
