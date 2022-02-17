using System.Xml.Serialization;

namespace JellySonic.Models;

/// <summary>
/// Response data interface.
/// </summary>
[XmlInclude(typeof(SubsonicError))]
public abstract class ResponseData
{
}
