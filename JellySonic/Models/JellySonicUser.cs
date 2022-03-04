using System;

namespace JellySonic.Models;

/// <summary>
/// JellySonic user for individual plugin configuration.
/// </summary>
public class JellySonicUser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JellySonicUser"/> class.
    /// </summary>
    public JellySonicUser()
    {
        JellyfinUserId = Guid.Empty;
        Password = string.Empty;
        Options = new JellySonicUserOptions();
    }

    /// <summary>
    /// Gets or sets Jellyfin User ID.
    /// </summary>
    public Guid JellyfinUserId { get; set; }

    /// <summary>
    /// Gets or sets user password.
    /// Used for Subsonic token authentication only.
    /// This is not password for Jellyfin user.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets user options.
    /// </summary>
    public JellySonicUserOptions Options { get; set; }
}

/// <summary>
/// JellySonic user options.
/// </summary>
public class JellySonicUserOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JellySonicUserOptions"/> class.
    /// </summary>
    public JellySonicUserOptions()
    {
        TokenAuthEnabled = true;
    }

    /// <summary>
    /// Gets or sets a value indicating whether user has enabled token authentication.
    /// </summary>
    public bool TokenAuthEnabled { get; set; }
}
