using System.Xml.Serialization;

namespace JellySonic.Models;

/// <summary>
/// A Subsonic error response class.
/// </summary>
public class Error : ResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    public Error()
    {
        ErrorCode = string.Empty;
        Message = ErrorCodes.Generic;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    /// <param name="message"> Error message.</param>
    /// <param name="errorCode">Error code. See <see cref="ErrorCodes"/> for predefined values.</param>
    public Error(string message, string errorCode)
    {
        ErrorCode = errorCode;
        Message = message;
    }

    /// <summary>
    /// Gets or sets error code.
    /// </summary>
    [XmlAttribute("code")]
    public string ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets error message.
    /// </summary>
    [XmlAttribute("message")]
    public string Message { get; set; }
}