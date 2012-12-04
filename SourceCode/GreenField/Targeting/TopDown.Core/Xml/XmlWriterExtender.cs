using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TopDown.Core.Xml
{
    public static class XmlWriterExtender
    {
        public static void WriteAttribute(this XmlWriter writer, String localName, String value)
        {
            writer.WriteStartAttribute(localName);
            writer.WriteString(value);
            writer.WriteEndAttribute();
        }
        public static void WriteAttribute(this XmlWriter writer, String localName, Int32 value)
        {
            writer.WriteStartAttribute(localName);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }
        public static void WriteElement(this XmlWriter writer, String localName, Action handler)
        {
            writer.WriteStartElement(localName);
            handler();
            writer.WriteEndElement();
        }
        public static void WriteElement(this XmlWriter writer, String localName, String namespaceUri, Action handler)
        {
            writer.WriteStartElement(localName, namespaceUri);
            handler();
            writer.WriteEndElement();
        }
    }
}
