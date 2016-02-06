using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyControl.Resources.Controls
{
    public enum LineStyleType
    {
        None,
        SolidLine,
        DashLine,
    }

    public enum LineShapeType
    {
        Polyline,
        Straight
    }

    public enum ArrowType
    {
        None,
        OneWay,
        TowWay
    }

    public class FlowPanel : Panel
    {
        double ArrowLength = 10d;
        Pen pen = null;

        enum RelativePos
        {
            SameRow,
            SameColumn
        }

        new enum FlowDirection
        {
            Vetical,
            Left,
            Right
        }

        public FlowPanel()
        {
            pen = new Pen(LineBrush, LineThickness);
            SetLineDashStyle();
        }

        public static readonly DependencyProperty ArrowAngleProperty = DependencyProperty.Register("ArrowAngle", typeof(double), typeof(FlowPanel), new UIPropertyMetadata(30d, new PropertyChangedCallback(ArrowAnglePropertyChangedCallback)));
        public double ArrowAngle
        {
            get { return (double)GetValue(ArrowAngleProperty); }
            set { SetValue(ArrowAngleProperty, value); }
        }
        static void ArrowAnglePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FlowPanel obj = sender as FlowPanel;
            obj.UpdateLayout();
        }

        public static readonly DependencyProperty ArrowTypeProperty = DependencyProperty.Register("ArrowType", typeof(ArrowType), typeof(FlowPanel), new UIPropertyMetadata(ArrowType.TowWay, new PropertyChangedCallback(ArrowTypePropertyChangedCallback)));
        public ArrowType ArrowType
        {
            get { return (ArrowType)GetValue(ArrowTypeProperty); }
            set { SetValue(ArrowTypeProperty, value); }
        }
        static void ArrowTypePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FlowPanel obj = sender as FlowPanel;
            obj.UpdateLayout();
        }

        public static readonly DependencyProperty LineShapeTypeProperty = DependencyProperty.Register("LineShapeType", typeof(LineShapeType), typeof(FlowPanel), new UIPropertyMetadata(LineShapeType.Straight, new PropertyChangedCallback(LineShapeTypePropertyChangedCallBack)));
        public LineShapeType LineShapeType
        {
            get { return (LineShapeType)GetValue(LineShapeTypeProperty); }
            set { SetValue(LineShapeTypeProperty, value); }
        }
        static void LineShapeTypePropertyChangedCallBack(object sender, DependencyPropertyChangedEventArgs e)
        {
            FlowPanel obj = sender as FlowPanel;
            obj.UpdateLayout();
        }

        public static readonly DependencyProperty LineStyleTypeProperty = DependencyProperty.Register("LineStyleType", typeof(LineStyleType), typeof(FlowPanel), new UIPropertyMetadata(LineStyleType.SolidLine, new PropertyChangedCallback(LineTypePropertyChangedCallback)));
        public LineStyleType LineStyleType
        {
            get { return (LineStyleType)GetValue(LineStyleTypeProperty); }
            set { SetValue(LineStyleTypeProperty, value); }
        }
        static void LineTypePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FlowPanel obj = sender as FlowPanel;
            obj.SetLineDashStyle();
        }

        private void SetLineDashStyle()
        {
            switch (LineStyleType)
            {
                case LineStyleType.SolidLine:
                    pen.DashStyle = new DashStyle();
                    break;
                case LineStyleType.DashLine:
                    pen.DashStyle = new DashStyle(new double[] { 2.5, 2.5 }, 0);
                    break;
            }

            this.UpdateLayout();
        }

        public static readonly DependencyProperty LineThicknessProperty = DependencyProperty.Register("LineThickness", typeof(float), typeof(FlowPanel), new UIPropertyMetadata(1f, new PropertyChangedCallback(LineThicknessPropertyChangedCallBack)));
        public float LineThickness
        {
            get { return (float)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }
        static void LineThicknessPropertyChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FlowPanel obj = sender as FlowPanel;
            obj.pen.Thickness = obj.LineThickness;
            obj.UpdateLayout();
        }

        public static readonly DependencyProperty LineBrushProperty = DependencyProperty.Register("LineBrush", typeof(Brush), typeof(FlowPanel), new UIPropertyMetadata(Brushes.Red, new PropertyChangedCallback(LineBrushPropertyChangedCallBack)));
        public Brush LineBrush
        {
            get { return (Brush)GetValue(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
        }
        static void LineBrushPropertyChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FlowPanel obj = sender as FlowPanel;
            obj.pen.Brush = obj.LineBrush;
            obj.UpdateLayout();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size desiredSize = new Size();
            Size panelSize = new Size(availableSize.Width, 0);

            double tempTotalWidth = 0;
            double tempCurRowHeight = 0;
            for (int index = 0; index < InternalChildren.Count; index++)
            {
                var child = InternalChildren[index];
                child.Measure(availableSize);
                desiredSize = child.DesiredSize;

                if (tempTotalWidth + child.DesiredSize.Width > panelSize.Width)//满足该条件 则对下一行进行统计
                {
                    panelSize.Height += tempCurRowHeight;
                    panelSize.Width = Math.Max(panelSize.Width, tempTotalWidth);
                    tempCurRowHeight = 0;
                    tempTotalWidth = 0;
                }

                tempTotalWidth += child.DesiredSize.Width;
                tempCurRowHeight = Math.Max(child.DesiredSize.Height, tempCurRowHeight);

                if (index == InternalChildren.Count - 1)
                {
                    panelSize.Height += tempCurRowHeight;
                    panelSize.Width = Math.Max(panelSize.Width, tempTotalWidth);
                }
            }


            MinWidth = desiredSize.Width;
            this.VerticalAlignment = VerticalAlignment.Stretch;

            availableSize = panelSize;

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Point childPos = new Point(0, 0);
            double NextChildY = 0;  //用于计算下一行Y位置   
            FlowDirection rowDirection = FlowDirection.Right;

            for (int index = 0; index < InternalChildren.Count; index++)
            {
                UIElement child = InternalChildren[index];

                if (rowDirection == FlowDirection.Right)
                {
                    if (childPos.X + child.DesiredSize.Width > finalSize.Width)
                    {
                        rowDirection = FlowDirection.Left;
                        childPos.Y = NextChildY;//下一行 该Child的Y位置 
                    }
                    else
                    {
                        child.Arrange(new Rect(childPos, new Size(child.DesiredSize.Width, child.DesiredSize.Height)));
                        NextChildY = Math.Max(child.DesiredSize.Height + childPos.Y, NextChildY);  // 记录本行所在位置的最大Y值，供下一行使用

                        childPos.X += child.DesiredSize.Width;

                        continue;
                    }
                }

                if (rowDirection == FlowDirection.Left)
                {
                    if (childPos.X - child.DesiredSize.Width < 0)
                    {
                        rowDirection = FlowDirection.Right;
                        childPos.Y = NextChildY;//下一行 该Child的Y位置
                        childPos.X = 0;//下一行 该Child的X位置
                        index--;
                    }
                    else
                    {
                        childPos.X -= child.DesiredSize.Width;
                        child.Arrange(new Rect(childPos, new Size(child.DesiredSize.Width, child.DesiredSize.Height)));
                        NextChildY = Math.Max(child.DesiredSize.Height + childPos.Y, NextChildY);  // 记录本行所在位置的最大Y值，供下一行使用

                        continue;
                    }
                }
            }

            return finalSize;
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (LineStyleType == LineStyleType.None)
            {
                base.OnRender(dc);
                return;
            }

            Point StartPoint = new Point();
            Point EndPoint = new Point();
            FlowDirection rowDirection = FlowDirection.Right;

            for (int index = 0; index < this.Children.Count; index++)
            {
                if (index == this.Children.Count - 1)
                    continue;

                UIElement CurChhild = this.Children[index];
                Vector CurV = VisualTreeHelper.GetOffset(CurChhild);

                UIElement NextChhild = this.Children[index + 1];
                Vector NextV = VisualTreeHelper.GetOffset(NextChhild);

                if (Math.Abs(CurV.Y - NextV.Y) >= CurChhild.RenderSize.Height) //该Child与下一个Child处于上下位置。
                {
                    StartPoint = new Point(CurV.X + CurChhild.RenderSize.Width / 2, CurV.Y + CurChhild.RenderSize.Height);
                    EndPoint = new Point(NextV.X + NextChhild.RenderSize.Width / 2, NextV.Y);

                    DrawingLine(dc, StartPoint, EndPoint, RelativePos.SameColumn, FlowDirection.Vetical);
                    rowDirection = rowDirection == FlowDirection.Right ? FlowDirection.Left : FlowDirection.Right;
                    continue;
                }

                double CurX = rowDirection == FlowDirection.Right ? CurV.X + CurChhild.RenderSize.Width : CurV.X;
                double NextX = rowDirection == FlowDirection.Right ? NextV.X : NextV.X + NextChhild.RenderSize.Width;

                StartPoint = new Point(CurX, CurV.Y + CurChhild.RenderSize.Height / 2);
                EndPoint = new Point(NextX, NextV.Y + NextChhild.RenderSize.Height / 2);

                DrawingLine(dc, StartPoint, EndPoint, RelativePos.SameRow, rowDirection);
            }

            base.OnRender(dc);
        }

        private void DrawingLine(DrawingContext dc, Point startPoint, Point endPoint, RelativePos relativePos, FlowDirection flowDirection)
        {
            if (LineShapeType == LineShapeType.Polyline)
            {
                DrawPolyline(dc, startPoint, endPoint, relativePos);
                DrawPolylineArrow(dc, startPoint, endPoint, relativePos, flowDirection);
            }
            else
            {
                DrawStraightLine(dc, startPoint, endPoint);
                DrawArrow(dc, startPoint, endPoint);
            }
        }

        #region 折线

        private void DrawPolyline(DrawingContext dc, Point startPoint, Point endPoint, RelativePos relativePos)
        {
            switch (relativePos)
            {
                case RelativePos.SameRow:
                    if (startPoint.Y == endPoint.Y)
                        dc.DrawLine(pen, startPoint, endPoint);
                    else
                    {
                        Point startPolyPoint = new Point((startPoint.X + endPoint.X) / 2, startPoint.Y);
                        Point endPolyPoint = new Point((startPoint.X + endPoint.X) / 2, endPoint.Y);

                        dc.DrawLine(pen, startPoint, startPolyPoint);
                        dc.DrawLine(pen, startPolyPoint, endPolyPoint);
                        dc.DrawLine(pen, endPolyPoint, endPoint);
                    }
                    break;
                case RelativePos.SameColumn:
                    if (startPoint.X == endPoint.X)
                        dc.DrawLine(pen, startPoint, endPoint);
                    else
                    {
                        Point startPolyPoint = new Point(startPoint.X, (startPoint.Y + endPoint.Y) / 2);
                        Point endPolyPoint = new Point(endPoint.X, (startPoint.Y + endPoint.Y) / 2);

                        dc.DrawLine(pen, startPoint, startPolyPoint);
                        dc.DrawLine(pen, startPolyPoint, endPolyPoint);
                        dc.DrawLine(pen, endPolyPoint, endPoint);
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        ///                  |\
        ///  对边(opposite)  | \  斜边(hypotenuse)
        ///    ______________|__\
        ///                  邻边(adjacent)
        ///                  
        /// </summary>
        private void DrawPolylineArrow(DrawingContext dc, Point startPoint, Point endPoint, RelativePos relativePos, FlowDirection flowDirection)
        {
            if (ArrowType == ArrowType.None)
                return;

            double oppositeLength = ArrowLength * Math.Sin(Math.PI * ArrowAngle / 180);
            double adjacentLength = ArrowLength * Math.Cos(Math.PI * ArrowAngle / 180);
            switch (relativePos)
            {
                case RelativePos.SameRow:

                    double endX = flowDirection == FlowDirection.Right ? endPoint.X - adjacentLength : endPoint.X + adjacentLength;
                    dc.DrawLine(pen, endPoint, new Point(endX, endPoint.Y - oppositeLength));
                    dc.DrawLine(pen, endPoint, new Point(endX, endPoint.Y + oppositeLength));

                    if (ArrowType == ArrowType.OneWay)
                        return;

                    double startX = flowDirection == FlowDirection.Right ? startPoint.X + adjacentLength : startPoint.X - adjacentLength;
                    dc.DrawLine(pen, startPoint, new Point(startX, startPoint.Y - oppositeLength));
                    dc.DrawLine(pen, startPoint, new Point(startX, startPoint.Y + oppositeLength));

                    break;
                case RelativePos.SameColumn:

                    dc.DrawLine(pen, endPoint, new Point(endPoint.X + oppositeLength, endPoint.Y - adjacentLength));
                    dc.DrawLine(pen, endPoint, new Point(endPoint.X - oppositeLength, endPoint.Y - adjacentLength));

                    if (ArrowType == ArrowType.OneWay)
                        return;

                    dc.DrawLine(pen, startPoint, new Point(startPoint.X + oppositeLength, startPoint.Y + adjacentLength));
                    dc.DrawLine(pen, startPoint, new Point(startPoint.X - oppositeLength, startPoint.Y + adjacentLength));

                    break;
            }
        }

        #endregion

        #region 直线

        private void DrawStraightLine(DrawingContext dc, Point startPoint, Point endPoint)
        {
            dc.DrawLine(pen, startPoint, endPoint);
        }

        private void DrawArrow(DrawingContext dc, Point startPoint, Point endPoint)
        {
            if (ArrowType == ArrowType.None)
                return;

            DrawArrawOnPoint(dc, startPoint, endPoint);

            if (ArrowType == ArrowType.OneWay)
                return;

            DrawArrawOnPoint(dc, endPoint, startPoint);
        }

        //在endPoint 上画箭头
        private void DrawArrawOnPoint(DrawingContext dc, Point startPoint, Point endPoint)
        {
            var matx = new Matrix();
            Vector endVect = startPoint - endPoint;
            //获取单位向量
            endVect.Normalize();
            endVect *= ArrowLength;
            //旋转夹角
            matx.Rotate(ArrowAngle);
            //计算上半段箭头的点
            Point upPoint = endPoint + endVect * matx;
            dc.DrawLine(pen, endPoint, upPoint);

            matx.Rotate(-2 * ArrowAngle);
            //计算下半段箭头的点
            Point downPoint = endPoint + endVect * matx;
            dc.DrawLine(pen, endPoint, downPoint);
        }

        #endregion
    }
}

