namespace JellySonic.Subsonic;

/// <summary>
/// Collection of almost all error codes defined in Subsonic API.
/// </summary>
public static class ErrorCodes
{
    /// <summary>
    /// A generic error.
    /// </summary>
    public const string Generic = "0";

    /// <summary>
    /// Required parameter is missing.
    /// </summary>
    public const string MissingParam = "10";

    /// <summary>
    /// Incompatible Subsonic REST protocol version. Client must upgrade.
    /// </summary>
    public const string IncompatibleClient = "20";

    /// <summary>
    /// Incompatible Subsonic REST protocol version. Server must upgrade.
    /// </summary>
    public const string IncompatibleServer = "30";

    /// <summary>
    /// Wrong username or password.
    /// </summary>
    public const string InvalidCredentials = "40";

    /// <summary>
    /// Token authentication not supported for LDAP users.
    /// </summary>
    public const string TokenNotSupportedForLDAP = "41";

    /// <summary>
    /// User is not authorized for the given operation.
    /// </summary>
    public const string NotAuthorized = "50";

    /// <summary>
    /// The requested data was not found.
    /// </summary>
    public const string DataNotFound = "70";
}
