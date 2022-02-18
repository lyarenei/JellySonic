using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Jellyfin.Data.Entities;
using JellySonic.Models;
using JellySonic.Services;
using MediaBrowser.Common.Extensions;
using MediaBrowser.Controller.Authentication;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.IO;
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
    }

    /// <summary>
    /// Authenticate user in Jellyfin.
    /// </summary>
    /// <returns>User authentication is successful.</returns>
    private User? AuthenticateUser()
    {
        var username = Request.Query["u"];
        var password = Request.Query["p"];

        try
        {
            var user = this._userManager.AuthenticateUser(
                    username,
                    password,
                    string.Empty,
                    HttpContext.GetNormalizedRemoteIp().ToString(),
                    true)
                .ConfigureAwait(false);

            return user.GetAwaiter().GetResult();
        }
        catch (AuthenticationException)
        {
            return null;
        }
    }

    // Taken from https://github.com/jellyfin/jellyfin-plugin-opds/blob/b78a8bcc979581fe92835235a2c0d59516b5df15/Jellyfin.Plugin.Opds/OpdsApi.cs#L294
    private FileStreamResult BuildOutput(SubsonicResponse subsonicResponse)
    {
        var memoryStream = new MemoryStream();
        var serializer = XmlHelper.Create(typeof(SubsonicResponse), SubsonicResponse.Namespace);
        using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, IODefaults.StreamWriterBufferSize, true))
        using (var textWriter = new XmlTextWriter(writer))
        {
            textWriter.Formatting = Formatting.Indented;
            var emptyNamespaces = new XmlSerializerNamespaces();
            emptyNamespaces.Add(string.Empty, string.Empty);
            serializer.Serialize(textWriter, subsonicResponse, emptyNamespaces);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        return File(memoryStream, "application/xml; charset=utf-8");
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
}
