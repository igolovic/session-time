using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SessionTime.SessionTimeCommon;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

namespace SessionTime.SessionTimeCommon
{
    public static class Utility
    {
        public static void Log(string logFilePath, string message)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(logFilePath, true))
            {
                file.WriteLine(message);
            }
        }

        public static void SerializeObjectToFile<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null) { return; }

            var settings = new XmlWriterSettings();
            settings.Indent = false;

            XmlDocument xmlDocument = new XmlDocument();
            XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, serializableObject);
                stream.Position = 0;
                xmlDocument.Load(stream);
                xmlDocument.Save(fileName);
                stream.Close();
            }
        }

        public static T DeserializeObjectFromFile<T>(string fileName)
        {
            T objectOut = default(T);
            if (!string.IsNullOrEmpty(fileName))
            {
                string attributeXml = string.Empty;

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(fileName);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                        reader.Close();
                    }

                    read.Close();
                }
            }
            return objectOut;
        }

        public static string SerializeObject<T>(T toSerialize)
        {
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(textWriter, settings))
            {
                xmlSerializer.Serialize(xmlWriter, toSerialize, ns);
                return textWriter.ToString();
            }
        }
    }
}