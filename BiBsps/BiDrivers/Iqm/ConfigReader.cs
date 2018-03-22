using System;
using System.Xml;

namespace BiBsps.BiDrivers.Iqm
{
    public class ConfigReader
    {
        private static XmlDocument xml = null;

        static ConfigReader()
        {
            xml = new XmlDocument();
            xml.Load(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ProductConfigs\IQMTOSA.xml"));
        }

        public static string GetItem(string item)
        {
            try
            {
                return xml.SelectSingleNode("CONFIG").SelectSingleNode(item).InnerText;
            }
            catch (Exception)
            {
                throw new Exception("Failed to get item " + item);
            }
        }
        public static string GetItem(string boardName, string item)
        {
            XmlNode boardNode = xml.SelectSingleNode("CONFIG").SelectSingleNode(boardName);
            if (boardNode != null)
            {
                return boardNode.SelectSingleNode(item).InnerText;
            }
            else
            {
                return GetItem(item);
            }
        }
        public static double ReadTOffset(int floor, int number, double temperature)
        {
            string level = temperature < 60 ? "Low" : (temperature < 100 ? "Mid" : "High");
            string select =
                @"CONFIG/TOffsetList/TOffset[@Floor='<floor>' and @Number='<number>' and @Level='<level>']".Replace("<floor>", floor.ToString())
                    .Replace("<number>", number.ToString())
                    .Replace("<level>", level);
            return double.Parse(xml.SelectSingleNode(select).InnerText);
        }
    }
}
