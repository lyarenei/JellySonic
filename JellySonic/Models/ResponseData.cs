using System.Xml.Serialization;

namespace JellySonic.Models;

/// <summary>
/// Response data interface.
/// </summary>
[XmlInclude(typeof(Error))]
public abstract class ResponseData
{
}
