using System.Xml.Serialization;
using JellySonic.Types;

namespace JellySonic.Models;

/// <summary>
/// A Subsonic error response class.
/// </summary>
public class SubsonicError : IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubsonicError"/> class.
    /// </summary>
    public SubsonicError()
    {
        Code = ErrorCodes.Generic;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubsonicError"/> class.
    /// </summary>
    /// <param name="message"> Error message.</param>
    /// <param name="code">Error code. See <see cref="ErrorCodes"/> for predefined values.</param>
    public SubsonicError(string? message, string code)
    {
        Message = message;
        Code = code;
    }

    /// <summary>
    /// Gets or sets error message.
    /// </summary>
    [XmlAttribute("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets error code.
    /// </summary>
    [XmlAttribute("code")]
    public string Code { get; set; }
}
