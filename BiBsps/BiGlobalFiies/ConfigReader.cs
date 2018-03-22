using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace BiBsps.BiGlobalFiies
{
    internal class ConfigReader
    {
        private static ConfigReader _inst;
        private readonly XmlDocument _xml;
        public static ConfigReader GetInstance(string fileName)
        {
            return _inst ?? (_inst = new ConfigReader(fileName));
        }
 

        private ConfigReader(string fileName)
        {
            _xml = new XmlDocument();
            _xml.Load(System.IO.Path.Combine(Environment.CurrentDirectory, fileName));  
        }

        public void GetItem(string item, out Dictionary<string, List<string>> items)
        {
            items = new Dictionary<string, List<string>>();
            var nodes = _xml.SelectSingleNode("Settings")?.SelectSingleNode(item)?.ChildNodes;
            if (nodes == null) return;
            foreach (XmlNode node in nodes)
            {
                var ls = (from XmlNode nd in node.ChildNodes select nd.InnerText).ToList();
                items[node.Name] = ls;
            }
        }

    }
}
