using System.Xml.Serialization;

namespace Fusion.Helpers;

/// <summary>
/// Provides helper methods for working with XML data, such as deserialization.
/// </summary>
public class XmlHelper
{
    /// <summary>
    /// Deserializes an XML string into an object of the specified type.
    /// </summary>
    /// <param name="xml">The XML string to deserialize.</param>
    /// <typeparam name="T">The type of the object to deserialize into.</typeparam>
    /// <returns>Returns an object of type T deserialized from the XML string.</returns>
    public static T FromXml<T>(string xml)
    {
        var serializer = new XmlSerializer(typeof(T));

        using var reader = new StringReader(xml);
        return (T)serializer.Deserialize(reader)!;
    }
}