using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace JellySonic.Formatters;

/// <summary>
/// Custom XML formatter.
/// </summary>
public class JellySonicXmlFormatter : XmlSerializerOutputFormatter
{
    /// <inheritdoc />
    protected override void Serialize(XmlSerializer xmlSerializer, XmlWriter xmlWriter, object? value)
    {
        var emptyNamespaces = new XmlSerializerNamespaces();
        emptyNamespaces.Add(string.Empty, string.Empty);
        xmlSerializer.Serialize(xmlWriter, value, emptyNamespaces);
    }
}
