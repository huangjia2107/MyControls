using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyControl.Resources.Controls
{
    public class PolylineGrid : Grid
    {
        public PolylineGrid()
        {

        }

        public static readonly DependencyProperty LineBrushProperty = DependencyProperty.Register("LineBrush", typeof(Brush), typeof(PolylineGrid), new UIPropertyMetadata(Brushes.Black));
        public Brush LineBrush
        {
            get { return (Brush)GetValue(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
        }

        /*  将相邻两点连线
        protected override void OnRender(DrawingContext dc)
        {
            if (InternalChildren.Count < 2)
            {
                base.OnRender(dc);
                return;
            }

            Point? StartPoint = null;
            Point? EndPoint = null;

            for (int index = 0; index < InternalChildren.Count; index++)
            {
                UIElement CurChhild = this.Children[index];
                Vector CurV = VisualTreeHelper.GetOffset(CurChhild);

                if (index == 0)
                    StartPoint = new Point(CurV.X + CurChhild.RenderSize.Width / 2, CurV.Y + CurChhild.RenderSize.Height / 2);
                else
                    EndPoint = new Point(CurV.X + CurChhild.RenderSize.Width / 2, CurV.Y + CurChhild.RenderSize.Height / 2);

                if (StartPoint != null && EndPoint != null)
                {
                    dc.DrawLine(new Pen(LineBrush, 1.0), StartPoint.Value, EndPoint.Value);
                    StartPoint = EndPoint;
                }
            } 
        }
        */

        // 由LineSegment连接相邻两点并最终构成Path
        protected override void OnRender(DrawingContext dc)
        {
            if (InternalChildren.Count < 2)
            {
                base.OnRender(dc);
                return;
            }

            PathSegmentCollection segmentCollection = new PathSegmentCollection();
            PathFigure pathFigure = new PathFigure() { Segments = segmentCollection }; 

            for (int index = 0; index < InternalChildren.Count; index++)
            {
                UIElement CurChhild = this.Children[index];
                Vector CurV = VisualTreeHelper.GetOffset(CurChhild);

                if (index == 0)
                    pathFigure.StartPoint = new Point(CurV.X + CurChhild.RenderSize.Width / 2, CurV.Y + CurChhild.RenderSize.Height / 2);
                else
                    segmentCollection.Add(new LineSegment() { Point = new Point(CurV.X + CurChhild.RenderSize.Width / 2, CurV.Y + CurChhild.RenderSize.Height / 2) });
            }
 
            PathGeometry pathGeometry = new PathGeometry() { Figures = new PathFigureCollection() { pathFigure } };
            dc.DrawGeometry(Brushes.Transparent, new Pen(LineBrush, 1.0), pathGeometry);
        }
        

        /* 由PolyLineSegment串联所有点并最终构成Path
        protected override void OnRender(DrawingContext dc)
        {
            if (InternalChildren.Count < 2)
            {
                base.OnRender(dc);
                return;
            }
            
            PolyLineSegment polylineSeg = new PolyLineSegment();
            PathSegmentCollection segmentCollection = new PathSegmentCollection() { polylineSeg};
            PathFigure pathFigure = new PathFigure() { Segments = segmentCollection };

            for (int index = 0; index < InternalChildren.Count; index++)
            {
                UIElement CurChhild = this.Children[index];
                Vector CurV = VisualTreeHelper.GetOffset(CurChhild);

                if (index == 0)
                    pathFigure.StartPoint = new Point(CurV.X + CurChhild.RenderSize.Width / 2, CurV.Y + CurChhild.RenderSize.Height / 2);
                else
                    polylineSeg.Points.Add(new Point(CurV.X + CurChhild.RenderSize.Width / 2, CurV.Y + CurChhild.RenderSize.Height / 2));
            }

            PathGeometry pathGeometry = new PathGeometry() { Figures = new PathFigureCollection() { pathFigure } };
            dc.DrawGeometry(Brushes.Transparent, new Pen(LineBrush, 1.0), pathGeometry);
        }
         * */
    }
}
