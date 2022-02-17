using JellySonic.Formatters;
using JellySonic.Subsonic;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JellySonic;

/// <summary>
/// The controller for JellySonic API.
/// </summary>
[ApiController]
[Route("subsonic")]
public class PluginController : ControllerBase
{
    private readonly ILogger<PluginController> _logger;
    private readonly IUserManager _userManager;
    private readonly IXmlSerializer _xmlSerializer;
    private readonly ILibraryManager _libraryManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginController"/> class.
    /// </summary>
    /// <param name="loggerFactory"> Logger factory.</param>
    /// <param name="userManager">User manager instance.</param>
    /// <param name="xmlSerializer">XML serializer instance.</param>
    /// <param name="libraryManager">Library manager instance.</param>
    public PluginController(ILoggerFactory loggerFactory, IUserManager userManager, IXmlSerializer xmlSerializer, ILibraryManager libraryManager)
    {
        _logger = loggerFactory.CreateLogger<PluginController>();
        _userManager = userManager;
        _xmlSerializer = xmlSerializer;
        _libraryManager = libraryManager;
    }

    /// <summary>
    /// Custom ObjectResult converter - adds XML formatter.
    /// </summary>
    /// <param name="val">Object to serialize.</param>
    /// <param name="status">HTTP status code.</param>
    /// <returns>Result object to use as response.</returns>
    private static ObjectResult ToObjectResult(object val, int status = StatusCodes.Status200OK)
    {
        var objectResult = new ObjectResult(val);
        objectResult.Formatters.Add(new JellySonicXmlFormatter());
        objectResult.StatusCode = status;
        return objectResult;
    }

    /// <summary>
    /// A simple HTTP Get endpoint. Some clients use it to check availability of the server instead of ping.
    /// </summary>
    /// <returns>Empty OK response.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Get()
    {
        return Ok();
    }

    /// <summary>
    /// Used to test connectivity with the server.
    /// </summary>
    /// <returns>Generic Subsonic response.</returns>
    [HttpGet]
    [Produces("application/xml")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("rest/ping")]
    public ActionResult Ping()
    {
        return ToObjectResult(new BaseResponse());
    }
}
