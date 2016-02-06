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
}
