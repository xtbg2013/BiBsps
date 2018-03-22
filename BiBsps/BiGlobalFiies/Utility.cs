using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace BiBsps.BiGlobalFiies
{
    public class Utility
    {
     
        private static readonly Dictionary<string, object> ObjDict = new Dictionary<string, object>();
       
        public static void Load<T>(string targetFile, out T subject)
        {
            if (File.Exists(targetFile))
            {
                using (var fstream = new FileStream(targetFile, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    subject = (T)serializer.Deserialize(fstream);
                }
            }
            else
            {
                subject = default(T);
            }
        }

        public static void Dump<T>(string targetFile, T subject)
        {
            if (ObjDict.ContainsKey(targetFile) == false)
                ObjDict[targetFile] = new object();
            lock (ObjDict[targetFile])
            {
                new FileStream(targetFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None).Close();
                using (var fstream = new FileStream(targetFile, FileMode.Truncate, FileAccess.Write, FileShare.None))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(fstream, subject);
                }
            }
        }

    }
}
