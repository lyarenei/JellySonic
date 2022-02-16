namespace JellySonic.Subsonic;

/// <summary>
/// Collection of almost all error codes defined in Subsonic API.
/// </summary>
public static class ErrorCodes
{
    /// <summary>
    /// A generic error.
    /// </summary>
    public const int Generic = 0;

    /// <summary>
    /// Required parameter is missing.
    /// </summary>
    public const int MissingParam = 10;

    /// <summary>
    /// Incompatible Subsonic REST protocol version. Client must upgrade.
    /// </summary>
    public const int IncompatibleClient = 20;

    /// <summary>
    /// Incompatible Subsonic REST protocol version. Server must upgrade.
    /// </summary>
    public const int IncompatibleServer = 30;

    /// <summary>
    /// Wrong username or password.
    /// </summary>
    public const int InvalidCredentials = 40;

    /// <summary>
    /// Token authentication not supported for LDAP users.
    /// </summary>
    public const int TokenNotSupportedForLDAP = 41;

    /// <summary>
    /// User is not authorized for the given operation.
    /// </summary>
    public const int NotAuthorized = 50;

    /// <summary>
    /// The requested data was not found.
    /// </summary>
    public const int DataNotFound = 70;
}
