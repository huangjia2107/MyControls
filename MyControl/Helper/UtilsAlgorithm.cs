using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Markup;

namespace MyControl.Helper
{
    /// <summary>
    /// 关闭所有对话框
    /// </summary>
    public class DialogCloser
    {
        public static void Execute()
        {
            // Enumerate windows to find dialogs
            EnumThreadWndProc callback = new EnumThreadWndProc(checkWindow);
            EnumThreadWindows(GetCurrentThreadId(), callback, IntPtr.Zero);
            GC.KeepAlive(callback);
        }

        private static bool checkWindow(IntPtr hWnd, IntPtr lp)
        {
            // Checks if <hWnd> is a Windows dialog
            StringBuilder sb = new StringBuilder(260);
            GetClassName(hWnd, sb, sb.Capacity);
            //#32770:对话框类名
            if (sb.ToString() == "#32770")
            {
                // Close it by sending WM_CLOSE to the window
                SendMessage(hWnd, 0x0010, IntPtr.Zero, IntPtr.Zero);
            }
            return true;
        }

        // P/Invoke declarations
        private delegate bool EnumThreadWndProc(IntPtr hWnd, IntPtr lp);
        [DllImport("user32.dll")]
        private static extern bool EnumThreadWindows(int tid, EnumThreadWndProc callback, IntPtr lp);
        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();
        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder buffer, int buflen);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
    }

    /// <summary>
    /// 屏幕键盘
    /// </summary>
    public class ScreenKeyboardCtr
    {
        private const Int32 WM_SYSCOMMAND = 274;
        private const UInt32 SC_CLOSE = 61536;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //显示屏幕键盘
        public static bool ShowInputPanel()
        {
            try
            {
                string path = @"C:\Program Files\Common Files\microsoft shared\ink\TabTip.exe";
                string path32 = @"C:\Program Files (x86)\Common Files\Microsoft Shared\Ink\TabTip32.exe";
                if (File.Exists(path))
                {
                    Process.Start(path);
                }
                else if (File.Exists(path32))
                {
                    Process.Start(path32);
                }
                else
                    return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //隐藏屏幕键盘
        public static void HideInputPanel()
        {
            IntPtr TouchhWnd = new IntPtr(0);
            TouchhWnd = FindWindow("IPTip_Main_Window", null);
            if (TouchhWnd == IntPtr.Zero)
                return;
            PostMessage(TouchhWnd, WM_SYSCOMMAND, SC_CLOSE, 0);
        }

        //Kill进程
        public static void CloseInputPanel()
        {
            try
            {
                System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("TabTip");
                foreach (System.Diagnostics.Process proc in processes)
                {
                    proc.Kill();
                }
            }
            catch { }
        }
    }

    /// <summary>
    /// 删除文件到回收站
    /// </summary>
    public class FileIOHelper
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public int wFunc;
            public string pFrom;
            public string pTo;
            public short fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        #region Dllimport

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);
        #endregion

        #region Const
        public const int FO_DELETE = 3;
        public const int FOF_ALLOWUNDO = 0x40;
        public const int FOF_NOCONFIRMATION = 0x10;
        #endregion

        #region Public Static Method
        public static void DeleteFileToRecyclebin(string file, Boolean showConfirmDialog)
        {
            try
            {
                var shf = new SHFILEOPSTRUCT();
                shf.wFunc = FO_DELETE;
                shf.fFlags = FOF_ALLOWUNDO;
                if (!showConfirmDialog)
                {
                    shf.fFlags |= FOF_NOCONFIRMATION;
                }
                shf.pFrom = file + '\0' + '\0';
                SHFileOperation(ref shf);
            }
            catch { }
        }
        #endregion

    }

    /// <summary>
    /// 针对面板的帮助类
    /// </summary>
    public class LayoutHelper
    {
        public static readonly DependencyProperty VisibilityProperty = DependencyProperty.RegisterAttached("Visibility", typeof(Visibility), typeof(LayoutHelper), new UIPropertyMetadata(Visibility.Visible, PropertyValueChangedCallback));
        public static Visibility GetVisibility(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(VisibilityProperty);
        }
        public static void SetVisibility(DependencyObject obj, Visibility value)
        {
            obj.SetValue(VisibilityProperty, value);
        }

        static void PropertyValueChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = sender as UIElement;
            if (e.NewValue == e.OldValue)
                return;

            Visibility visibility = (Visibility)(e.NewValue);
            element.Visibility = visibility;

            DependencyObject parentNode = VisualTreeHelper.GetParent(element);
            while (parentNode != null)
            {
                //可以添加Child的控件
                if (parentNode is FrameworkElement && parentNode is IAddChild)
                {
                    if (parentNode is Panel)
                        visibility = GetAndSetParentVisibility<Panel>(parentNode as Panel);
                    else if (parentNode is Decorator)
                        visibility = GetAndSetParentVisibility<Decorator>(parentNode as Decorator);
                }
                else
                {
                    //终止于ContentControl
                    if (parentNode is ContentPresenter)
                    {
                        ContentControl obj = (ContentControl)(parentNode as ContentPresenter).TemplatedParent;
                        obj.Visibility = visibility;
                        break;
                    }
                }
                parentNode = VisualTreeHelper.GetParent(parentNode);
            }
        }

        private static Visibility GetAndSetParentVisibility<T>(T obj) where T : FrameworkElement, IAddChild
        {
            Visibility result = Visibility.Collapsed;
            if (obj is Panel)
            {
                var panel = obj as Panel;
                foreach (var child in panel.Children)
                {
                    result = (child as UIElement).Visibility;
                    if (result == Visibility.Visible)
                        break;
                }
                panel.Visibility = result;
            }
            else if (obj is Decorator)
            {
                var decorator = obj as Decorator;
                result = decorator.Visibility = decorator.Child.Visibility;
            }

            return result;
        }
    }
    
    public class PopupBehaviour
    {
        public static readonly DependencyProperty TopmostProperty = DependencyProperty.RegisterAttached("Topmost", typeof(bool), typeof(PopupBehaviour), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(TopmostPropertyChangedCallback)));
        public static bool GetTopmost(DependencyObject obj)
        {
            return (bool)obj.GetValue(TopmostProperty);
        }
        public static void SetTopmost(DependencyObject obj, bool value)
        {
            obj.SetValue(TopmostProperty, value);
        } 

        static void TopmostPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Popup obj = sender as Popup;
            obj.Opened += new EventHandler(obj_Opened); 
        }

        static void obj_Opened(object sender, EventArgs e)
        {
            Popup obj = sender as Popup;
           
            var hwnd = ((HwndSource)PresentationSource.FromVisual(obj.Child)).Handle;
            RECT rect;

            if (GetWindowRect(hwnd, out rect))
            {
                SetWindowPos(hwnd, PopupBehaviour.GetTopmost(obj) ? -1 : -2, rect.Left, rect.Top, (int)obj.Width, (int)obj.Height, 0);
            }
        } 

        #region P/Invoke imports & definitions

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hWnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        #endregion

    }
}
