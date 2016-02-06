using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;

namespace MyControl.Helper
{
    public class MyControlLog
    {
        public ILog Log = null;

        private static bool bConfigLog = false;
        private const string strLoggerName = "MyControlLogger";

        private void CheckAndConfigLogger()
        {
            string strConfigFileFullPath = ResourceMap.MyControlPathHashtable[MyControlPath.LogFileFullPath] as string;
            if(!bConfigLog)
            {
                bConfigLog = true;
                FileInfo configFileInfo = new FileInfo(strConfigFileFullPath);
                XmlConfigurator.ConfigureAndWatch(configFileInfo); 
            }
        }

        public MyControlLog()
        {
            CheckAndConfigLogger();
            Log = LogManager.GetLogger(strLoggerName);
        } 
    }
}
