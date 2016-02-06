using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using SWM = System.Windows.Media;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Security;

namespace MyControl.Helper
{
    public class GraphicAlgorithm
    {
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, uint Length);

        private sealed class SafeHBitmapHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            [SecurityCritical]
            public SafeHBitmapHandle(Bitmap bitmap)
                : base(true)
            {
                SetHandle(bitmap.GetHbitmap());
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            protected override bool ReleaseHandle()
            {
                return DeleteObject(handle);
            }
        }

        public static FormatConvertedBitmap GetFormatConvertedBitmap(SWM.PixelFormat pf, BitmapSource bs)
        {
            FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();
            newFormatedBitmapSource.BeginInit();
            newFormatedBitmapSource.Source = bs;
            newFormatedBitmapSource.DestinationFormat = pf;
            newFormatedBitmapSource.EndInit();

            return newFormatedBitmapSource;
        }

        public static Bitmap RGBBytesToBitmap(byte[] rgbData, int imageWidth, int imageHeight)
        {
            Bitmap bitmap = new Bitmap(imageWidth, imageHeight, PixelFormat.Format24bppRgb);
            BitmapData bmpData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            // Marshal.Copy(rgbData, 0, bmpData.Scan0, rgbData.Length);

            if (bmpData.Stride == imageWidth * 3)
            {
                Marshal.Copy(rgbData, 0, bmpData.Scan0, imageWidth * imageHeight * 3);
            }
            else
            {
                for (int i = 0; i < bitmap.Height; i++)
                {
                    IntPtr p = new IntPtr(bmpData.Scan0.ToInt32() + bmpData.Stride * i);
                    Marshal.Copy(rgbData, i * bitmap.Width * 3, p, bitmap.Width * 3);
                }
            }

            bitmap.UnlockBits(bmpData);
            return bitmap;
        }

        public static BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSource result = null;

            try
            {
                result = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                //Release resource
                DeleteObject(hBitmap);
            }

            return result;
        }

        public static BitmapSource ConvertBitmapToBitmapSourceBySaveHandle(Bitmap bitmap)
        {
            using (var handle = new SafeHBitmapHandle(bitmap))
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(handle.DangerousGetHandle(),
                    IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
        }

        public static BitmapSource ConvertNativePointerToBitmapSource(IntPtr pData, int w, int h, SWM.PixelFormat pixelFormat)
        {
            int ch = 1;

            if (pixelFormat == SWM.PixelFormats.Gray8) ch = 1; //grey scale image 0-255
            if (pixelFormat == SWM.PixelFormats.Bgr24) ch = 3; //RGB
            if (pixelFormat == SWM.PixelFormats.Bgr32) ch = 4; //RGB + alpha

            WriteableBitmap wbm = new WriteableBitmap(w, h, 96, 96, pixelFormat, null);
            CopyMemory(wbm.BackBuffer, pData, (uint)(w * h * ch));

            wbm.Lock();
            wbm.AddDirtyRect(new Int32Rect(0, 0, wbm.PixelWidth, wbm.PixelHeight));
            wbm.Unlock();

            return wbm;
        }

        public static BitmapSource ConvertArrayToBitmapSource(byte[] data, int w, int h, SWM.PixelFormat pixelFormat)
        {
            int ch = 1;

            if (pixelFormat == SWM.PixelFormats.Gray8) ch = 1; //grey scale image 0-255
            if (pixelFormat == SWM.PixelFormats.Bgr24) ch = 3; //RGB
            if (pixelFormat == SWM.PixelFormats.Bgr32) ch = 4; //RGB + alpha

            WriteableBitmap wbm = new WriteableBitmap(w, h, 96, 96, pixelFormat, null);
            wbm.WritePixels(new Int32Rect(0, 0, w, h), data, ch * w, 0);

            return wbm;
        }

        public static BitmapImage ConvertBtyeArrayToBitmapImage(byte[] byteArray)
        {
            BitmapImage bmpi = null;
            try
            {
                bmpi = new BitmapImage();
                bmpi.BeginInit();
                bmpi.StreamSource = new MemoryStream(byteArray);
                bmpi.EndInit();
            }
            catch
            {
                bmpi = null;
            }

            return bmpi;
        }
        
        public static SWM.Brush ConvertStrToBrush(string brushString)
        {
            return new SWM.SolidColorBrush((SWM.Color)SWM.ColorConverter.ConvertFromString(brushString));
        }

        public static bool SaveBitmapToImgFile(string filePathWithoutExtension, Bitmap bitmap, ImageFormat imgFormat)
        {
            if (string.IsNullOrEmpty(filePathWithoutExtension) || bitmap == null)
                return false;

            filePathWithoutExtension += "." + ResourceMap.ImageFormatExtensionHashtable[imgFormat];
            try
            {
                bitmap.Save(filePathWithoutExtension, imgFormat);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (bitmap != null)
                    bitmap.Dispose();
            }
        }

        //vsual 可以是 FrameworkElement 
        public static bool SaveRenderTargetBitmapToImgFile(string filePathWithoutExtension, SWM.Visual vsual, int widhth, int height, ImageFormat imgFormat)
        {
            if (vsual == null)
                return false;

            string filePath = filePathWithoutExtension + "." + ResourceMap.ImageFormatExtensionHashtable[imgFormat];
            BitmapEncoder encoder = ResourceMap.ImageFormatBitmapEncoderHashtable[imgFormat] as BitmapEncoder;

            if (encoder == null)
                return false;

            RenderTargetBitmap rtb = null;
            BitmapFrame bf = null;
            try
            {
                rtb = RenderVisaulToBitmap(vsual, widhth, height);
                bf = BitmapFrame.Create(rtb);
                encoder.Frames.Add(bf);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    encoder.Save(fileStream);
                }

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                encoder = null;
                bf = null;
                rtb = null;
            }
        }

        //vsual 可以是 FrameworkElement 
        public static RenderTargetBitmap RenderVisaulToBitmap(SWM.Visual vsual, int width, int height)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, SWM.PixelFormats.Rgb24);
            rtb.Render(vsual);

            return rtb;
        }
    }
}
