using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JellySonic.Controllers;

/// <summary>
/// Common plugin controller.
/// </summary>
[ApiController]
[Route("subsonic")]
public class JellySonicController : ControllerBase
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="JellySonicController"/> class.
    /// </summary>
    /// <param name="loggerFactory">Logger factory.</param>
    public JellySonicController(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<JellySonicController>();
    }

    /// <summary>
    /// A simple HTTP Get endpoint.
    /// Some clients use it to check availability of the server instead of ping.
    /// </summary>
    /// <returns>Empty OK response.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        this._logger.LogInformation("called basic get");
        _logger.LogDebug("plain get endpoint was called");
        return Ok();
    }
}
