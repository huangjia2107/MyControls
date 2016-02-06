using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyControl.Helper
{
    public enum RWDATA_RESULT
    {
        FILE_READSUCCESS,
        FILE_WRITESUCCESS,
        FILE_CREATEFAILURE,
        FILE_OPENFAILURE,
        FILE_NOTEXIST,
        FILE_READFAILURE,
        FILE_WRITEFAILURE,
        PARAM_NULL,
        UNKNOWN
    }

    public enum CursorValue
    {
        Arrow = 65539,
        Cross = 65545,
        SizeAll = 65557,
        SizeNWSE = 65549,
        SizeNESW = 65551,
        SizeWE = 65553,
        SizeNS = 65555,
        Wait = 65543
    }

    public enum MyControlPath
    {
        SettingFolderFullPath,
        SettingFileFullPath, 
        LogFileFullPath, 
        ConfigFileFullPath
    }
}
