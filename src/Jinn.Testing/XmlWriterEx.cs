using System.Xml;

namespace Jinn.Testing;

public sealed class XmlWriterEx : IDisposable
{
    private readonly XmlWriter _writer;

    public bool SuppressSyntaxComments { get; set; }

    private XmlWriterEx(XmlWriter writer)
    {
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    public static XmlWriterEx Create(Utf8StringWriter writer)
    {
        return new XmlWriterEx(
            XmlWriter.Create(
                writer,
                new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = true,
                }));
    }

    public void Dispose()
    {
        _writer.Dispose();
    }

    public void WriteElement(string name, Action action)
    {
        _writer.WriteStartElement(name);
        action();
        _writer.WriteEndElement();
    }

    public void WriteElement(string name, string? value)
    {
        _writer.WriteElementString(name, value);
    }

    public void WriteAttribute<T>(string name, T value)
    {
        _writer.WriteAttributeString(name, value?.ToString());
    }

    internal void WriteComment(string text)
    {
        _writer.WriteComment(text);
    }
}