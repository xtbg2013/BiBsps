using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BILib
{
    public class SettingsReader
    {
        private static SettingsReader inst = null;
        private XmlDocument xml = null;

        public static SettingsReader GetInstance()
        {
            if(inst==null)
                inst = new SettingsReader();
            return inst;
        }

        private SettingsReader()
        {
            this.xml = new XmlDocument();
            xml.Load(System.IO.Path.Combine(Environment.CurrentDirectory, @"configurations\Settings.xml"));
        }

        public string GetSettingsValue(string firstLevelName, string secondLevelName)
        {
            foreach (XmlElement element in xml.DocumentElement.ChildNodes)
                if (element.Name == firstLevelName)
                    try
                    {
                        return element.SelectSingleNode(secondLevelName).InnerText;
                    }
                    catch 
                    {
                        return "";
                    }
            return "";
        }
    }
}
