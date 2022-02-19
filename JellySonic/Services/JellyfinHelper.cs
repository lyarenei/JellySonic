using Jellyfin.Data.Entities;
using MediaBrowser.Controller.Authentication;
using MediaBrowser.Controller.Library;
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
}
