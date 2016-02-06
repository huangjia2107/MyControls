using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

namespace MyControl.Helper
{
    class ResourceMap
    {
        public static Hashtable MyControlPathHashtable = new Hashtable()
        { 
            {MyControlPath.LogFileFullPath,                     AppDomain.CurrentDomain.BaseDirectory+"Log4NetConfig.xml"},
        };
        
        public static Hashtable ImageFormatExtensionHashtable = new Hashtable()
        {
            {ImageFormat.Jpeg,              "jpeg"},
            {ImageFormat.Gif,               "gif"},
            {ImageFormat.Png,               "png"},
            {ImageFormat.Bmp,               "bmp"},
            {ImageFormat.Emf,               "emf"},
            {ImageFormat.Exif,              "exif"},
            {ImageFormat.Icon,              "icon"},
            {ImageFormat.Tiff,              "tiff"},
            {ImageFormat.Wmf,               "wmf"},
        };

        public static Hashtable ImageFormatBitmapEncoderHashtable = new Hashtable()
        {
            {ImageFormat.Jpeg,              new JpegBitmapEncoder()},
            {ImageFormat.Gif,               new GifBitmapEncoder()},
            {ImageFormat.Png,               new PngBitmapEncoder()},
            {ImageFormat.Bmp,               new BmpBitmapEncoder()},
            {ImageFormat.Tiff,              new TiffBitmapEncoder()},
        };
    }
}
