using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;

namespace MyControl.Helper
{
    class DragDropAdorner : Adorner
    {
        Point _posRelative;
        public DragDropAdorner(UIElement parent,Point posRelative)
            : base(parent)
        {
            Debug.WriteLine("Adorner Construct.");
            IsHitTestVisible = false; // Seems Adorner is hit test visible?
            _posRelative = posRelative;
            mDraggedElement = parent as FrameworkElement; 
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (mDraggedElement != null)
            {
                Win32.POINT screenPos = new Win32.POINT();
                if (Win32.GetCursorPos(ref screenPos))
                {
                    Point pos = mDraggedElement.PointFromScreen(new Point(screenPos.X, screenPos.Y));

                    Debug.WriteLine("screenPos.X:" + screenPos.X + "  screenPos.Y:" + screenPos.Y + "  pos.X:" + pos.X + "  pos.Y:" + pos.Y + "  _posRelative.X:" + _posRelative.X + "  _posRelative.Y:" + _posRelative.Y);
                    Rect rect = new Rect(
                        pos.X - _posRelative.X, 
                        pos.Y - _posRelative.Y, 
                        mDraggedElement.ActualWidth, 
                        mDraggedElement.ActualHeight);

                    drawingContext.DrawRectangle(new VisualBrush(mDraggedElement), new Pen(Brushes.Transparent, 0), rect);
                }
            }
        }

        FrameworkElement mDraggedElement = null; 
    }

    public static class Win32
    {
        public struct POINT { public Int32 X; public Int32 Y; }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref POINT point);
    }
}