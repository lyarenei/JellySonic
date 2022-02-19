using Jellyfin.Data.Entities;
using Jellyfin.Data.Enums;
using JellySonic.Models;
using JellySonic.Services;
using MediaBrowser.Common.Extensions;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Querying;
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
    private readonly IUserManager _userManager;
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
        _userManager = userManager;
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
            var err = new ErrorResponseData("invalid credentials", ErrorCodes.NotAuthorized);
            return BuildOutput(new SubsonicResponse() { ResponseData = err });
        }

        var artistId = Request.Query["id"];
        var artist = _libraryManager.GetItemById(artistId);
        if (artist == null)
        {
            var err = new ErrorResponseData("artist not found", ErrorCodes.DataNotFound);
            return BuildOutput(new SubsonicResponse() { ResponseData = err });
        }

        var query = new InternalItemsQuery
        {
            IncludeItemTypes = new[] { BaseItemKind.MusicAlbum },
            AlbumArtistIds = new[] { artist.Id },
            OrderBy = new (string, SortOrder)[] { (ItemSortBy.SortName, SortOrder.Ascending) },
            Recursive = true
        };

        query.SetUser(user);

        var albums = _libraryManager.GetItemList(query);
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
            var err = new ErrorResponseData("invalid credentials", ErrorCodes.NotAuthorized);
            return BuildOutput(new SubsonicResponse { ResponseData = err });
        }

        var query = new InternalItemsQuery
        {
            OrderBy = new (string, SortOrder)[] { (ItemSortBy.SortName, SortOrder.Ascending) },
            Recursive = true
        };

        query.SetUser(user);
        var queryData = _libraryManager.GetAlbumArtists(query);
        if (queryData?.Items == null)
        {
            var err = new ErrorResponseData("no artists found", ErrorCodes.DataNotFound);
            return BuildOutput(new SubsonicResponse { ResponseData = err });
        }

        var artists = new ArtistsResponseData(queryData.Items);
        return BuildOutput(new SubsonicResponse { ResponseData = artists });
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
}
