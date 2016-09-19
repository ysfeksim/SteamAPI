using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SteamAPI
{
    public class DataSave
    {
        /// <summary>
        /// Save JSON data to an XML file
        /// </summary>
        /// <param name="json"></param>
        /// <param name="dataName"></param>
        public static void toXml(string json, string dataName)
        {
            XmlDocument doc = JsonConvert.DeserializeXmlNode(json);
            doc.Save(dataName + ".xml");
        }
        public static void WriteXML<T>(List<T> obj, Type class_type)
        {
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(class_type);

            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SerializationOverview.xml";
            System.IO.FileStream file = System.IO.File.Create(path);
            foreach (var item in obj)
            {
                writer.Serialize(file, item);
            }
            file.Close();
        }
    }
}
