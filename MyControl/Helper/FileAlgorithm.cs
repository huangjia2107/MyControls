using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace MyControl.Helper
{
    public class FileAlgorithm
    {
        public static RWDATA_RESULT WriteDataToFile_XML<T>(string strFilePath, T Parameter)
        {
            if (Parameter == null)
                return RWDATA_RESULT.PARAM_NULL;

            using (FileStream fs = new FileStream(strFilePath, FileMode.Create, FileAccess.Write))
            {
                if (fs == null)
                    return RWDATA_RESULT.FILE_CREATEFAILURE;

                XmlSerializer s = new XmlSerializer(typeof(T));
                try
                {
                    s.Serialize(fs, Parameter);
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return RWDATA_RESULT.FILE_WRITEFAILURE;
                }
                return RWDATA_RESULT.FILE_WRITESUCCESS;
            }
        }

        public static RWDATA_RESULT ReadDataFromFile_XML<T>(string strFilePath, ref T Parameter)
        {
            if (!File.Exists(strFilePath))
                return RWDATA_RESULT.FILE_NOTEXIST;

            using(FileStream fs = new FileStream(strFilePath, FileMode.Open, FileAccess.Read))
            {
                if (fs == null)
                    return RWDATA_RESULT.FILE_OPENFAILURE;

                XmlSerializer s = new XmlSerializer(typeof(T));
                try
                {
                    Parameter = (T)s.Deserialize(fs);
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return RWDATA_RESULT.FILE_READFAILURE;
                }
                return RWDATA_RESULT.FILE_READSUCCESS;
            }
        }
    }
    
    public static class XmlSerializerHelper
    { 
        private static Dictionary<Type, XmlSerializer> _cache; 

        static XmlSerializerHelper()
        {  
            _cache = new Dictionary<Type, XmlSerializer>();
        } 

        public static XmlSerializer GetSerializer<T>()
        {
            var type = typeof(T);
            if (!_cache.ContainsKey(type))
            {
                _cache.Add(type, XmlSerializer.FromTypes(new[] { type }).FirstOrDefault()); 
            }

            return _cache[type];
        } 
    }
    
    public static void AppendXmlDataToFile<T>(string strFilePath, T Parameter)
        {
            using (Stream fs = new FileStream(strFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = true
                };
                using (XmlWriter xw = XmlWriter.Create(fs, settings))
                {
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);

                    XmlSerializer xs = new XmlSerializer(Parameter.GetType());
                    xs.Serialize(xw, Parameter, namespaces);
                }

                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write("\r\n");
                }
            }
        }

        public static T ConvertXmlNode<T>(XmlNode node) where T : class
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (var stringReader = new StringReader(node.OuterXml))
            {
                return (T)xs.Deserialize(stringReader);
            }
        }

        public static T ReadSpecifiedNoteDataFromFile<T>(string strFilePath, Predicate<T> predicate) where T : class
        {
            if (!File.Exists(strFilePath))
                return null;

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml("<?xml version=\"1.0\"?><Root>" + File.ReadAllText(strFilePath) + @"</Root>");

            foreach (XmlNode xmlNode in xmldoc.SelectSingleNode("//Root").ChildNodes)
            {
                try
                {
                    T t = ConvertXmlNode<T>(xmlNode);
                    if (predicate(t))
                        return t;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message + ex.StackTrace);
                }
            }

            return null;
        }

        public static List<T> ReadLatestSpecifiedCountXmlDataFromFile<T>(string strFilePath, int count) where T : class
        {
            List<T> tList = new List<T>();
            if (!File.Exists(strFilePath))
                return tList;

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml("<?xml version=\"1.0\"?><Root>" + File.ReadAllText(strFilePath) + @"</Root>");

            XmlNode currentXmlNode = xmldoc.SelectSingleNode("//Root").LastChild;
            while (tList.Count < count)
            {
                if (currentXmlNode == null)
                    break;

                tList.Add(ConvertXmlNode<T>(currentXmlNode));
                currentXmlNode = currentXmlNode.PreviousSibling;
            }

            return tList;
        }

        public static List<T> ReadAllNoteDataFromFile<T>(string strFilePath) where T : class
        {
            if (!File.Exists(strFilePath))
                return null;

            try
            {
                List<T> tList = new List<T>();

                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml("<?xml version=\"1.0\"?><Root>" + File.ReadAllText(strFilePath) + @"</Root>");

                XmlNode currentXmlNode = xmldoc.SelectSingleNode("//Root").FirstChild;

                while (true)
                {
                    if (currentXmlNode == null)
                        break;

                    tList.Add(ConvertXmlNode<T>(currentXmlNode));
                    currentXmlNode = currentXmlNode.NextSibling;
                }

                return tList;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("[ ReadAllNoteDataFromFile ] Exception : " + ex.Message + ex.StackTrace);
                return null;
            }
        }

        public static List<T> ReadAllNoteDataFromFileContent<T>(string fileContent) where T : class
        {
            if (string.IsNullOrEmpty(fileContent))
                return null;

            try
            {
                List<T> tList = new List<T>();

                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(("<?xml version=\"1.0\"?><Root>" + fileContent + @"</Root>").Replace("\0", ""));

                XmlNode currentXmlNode = xmldoc.SelectSingleNode("//Root").FirstChild;

                while (true)
                {
                    if (currentXmlNode == null)
                        break;

                    tList.Add(ConvertXmlNode<T>(currentXmlNode));
                    currentXmlNode = currentXmlNode.NextSibling;
                }

                return tList;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("[ ReadAllNoteDataFromFile ] Exception : " + ex.Message + ex.StackTrace);
                return null;
            }
        }
}
