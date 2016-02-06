using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;

namespace MyControl.Resources.Controls
{
    public enum AnchorLocation
    {
        None,
        LeftTop,
        LeftBottom,
        TopLeft,
        TopRight,
        RightTop,
        RightBottom,
        BottomLeft,
        BottomRight
    }

    public class BalloonBorder : Decorator
    {
        private AnchorLocation AnchorLocation { get; set; }
        enum BorderSizeType
        {
            CombinedSize,
            OnlyAnchorSize
        }

        public BalloonBorder()
            : base()
        {
            AnchorLocation = GetAnchorLocation();
        }

        new private double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        new private double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public static readonly DependencyProperty AnchorPointProperty = DependencyProperty.Register("AnchorPoint", typeof(Point), typeof(BalloonBorder), new FrameworkPropertyMetadata(new Point(-10, 10),
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange |
            FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(UpdateLayoutPropertyChangedCallBack)));
        public Point AnchorPoint
        {
            get { return (Point)GetValue(AnchorPointProperty); }
            set { SetValue(AnchorPointProperty, value); }
        }

        static void UpdateLayoutPropertyChangedCallBack(object sender, DependencyPropertyChangedEventArgs e)
        {
            BalloonBorder obj = sender as BalloonBorder;
            obj.AnchorLocation = obj.GetAnchorLocation();
            obj.UpdateLayout();
        }

        public static readonly DependencyProperty BackgroundProperty = Panel.BackgroundProperty.AddOwner(typeof(BalloonBorder), new FrameworkPropertyMetadata(Brushes.Transparent,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(BalloonBorder), new FrameworkPropertyMetadata(default(CornerRadius),
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(BalloonBorder), new FrameworkPropertyMetadata(Brushes.Black,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(double), typeof(BalloonBorder), new FrameworkPropertyMetadata(0.5d,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
        public double BorderThickness
        {
            get { return (double)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(BalloonBorder), new FrameworkPropertyMetadata(default(Thickness),
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(UpdateLayoutPropertyChangedCallBack)));
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public static readonly DependencyProperty AnchorWidthProperty = DependencyProperty.Register("AnchorWidth", typeof(double), typeof(BalloonBorder), new FrameworkPropertyMetadata(200d,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
        public double AnchorWidth
        {
            get { return (double)GetValue(AnchorWidthProperty); }
            set { SetValue(AnchorWidthProperty, value); }
        }

        public static readonly DependencyProperty AnchorHeightProperty = DependencyProperty.Register("AnchorHeight", typeof(double), typeof(BalloonBorder), new FrameworkPropertyMetadata(100d,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
        public double AnchorHeight
        {
            get { return (double)GetValue(AnchorHeightProperty); }
            set { SetValue(AnchorHeightProperty, value); }
        }

        #region Public

        private static Size HelperCollapseThickness(Thickness th)
        {
            return new Size(th.Left + th.Right, th.Top + th.Bottom);
        }

        private static Size HelperCollapseBorder(double db)
        {
            return new Size(db * 2, db * 2);
        }

        private AnchorLocation GetAnchorLocation()
        {
            double wToH = AnchorWidth / AnchorHeight;

            //Middle
            if (AnchorPoint.X <= AnchorWidth && AnchorPoint.X >= 0 && AnchorPoint.Y <= AnchorHeight && AnchorPoint.Y >= 0)
            {
                return AnchorLocation.None;
            }

            //LeftTop
            if (AnchorPoint.X < 0 && AnchorPoint.Y <= AnchorHeight / 2 && AnchorPoint.X <= AnchorPoint.Y * wToH)
            {
                return AnchorLocation.LeftTop;
            }

            //LeftBottom
            if (AnchorPoint.X < 0 && AnchorPoint.Y > AnchorHeight / 2 && AnchorPoint.X <= AnchorWidth - wToH * AnchorPoint.Y)
            {
                return AnchorLocation.LeftBottom;
            }

            //TopLeft
            if (AnchorPoint.Y < 0 && AnchorPoint.X <= AnchorWidth / 2 && AnchorPoint.X > AnchorPoint.Y * wToH)
            {
                return AnchorLocation.TopLeft;
            }

            //TopRight
            if (AnchorPoint.Y < 0 && AnchorPoint.X > AnchorWidth / 2 && AnchorPoint.X < AnchorWidth - wToH * AnchorPoint.Y)
            {
                return AnchorLocation.TopRight;
            }

            //RightTop
            if (AnchorPoint.X > AnchorWidth && AnchorPoint.X >= AnchorWidth - wToH * AnchorPoint.Y && AnchorPoint.Y <= AnchorHeight / 2)
            {
                return AnchorLocation.RightTop;
            }

            //RightBottom
            if (AnchorPoint.X > AnchorWidth && AnchorPoint.X >= wToH * AnchorPoint.Y && AnchorPoint.Y > AnchorHeight / 2)
            {
                return AnchorLocation.RightBottom;
            }

            //BottomRight
            if (AnchorPoint.X >= AnchorWidth / 2 && AnchorPoint.X < wToH * AnchorPoint.Y && AnchorPoint.Y > AnchorHeight)
            {
                return AnchorLocation.BottomRight;
            }

            //BottomLeft
            if (AnchorPoint.X > AnchorWidth - wToH * AnchorPoint.Y && AnchorPoint.X < AnchorWidth / 2 && AnchorPoint.Y > AnchorHeight)
            {
                return AnchorLocation.BottomLeft;
            }

            return AnchorLocation.None;
        }

        private Size GetSizeForDecorator(BorderSizeType type)
        {
            Size tempSize = new Size(0, 0);

            if (type == BorderSizeType.CombinedSize)//获取“边”，“Anchor”及“Padding”所占空间大小
            {
                Size border = HelperCollapseBorder(BorderThickness);
                Size padding = HelperCollapseThickness(Padding);

                tempSize = new Size(border.Width + padding.Width, border.Height + padding.Height);  //获取“边”及“Padding”所占空间大小
            }

            // 获取“Anchor”所占空间大小
            double tempHeight = 0;
            double tempWidth = 0;
            switch (AnchorLocation)
            {
                case AnchorLocation.None:

                    break;
                case AnchorLocation.TopLeft:
                    tempWidth = AnchorPoint.X < 0 ? tempSize.Width + Math.Abs(AnchorPoint.X) : tempSize.Width;
                    tempSize = new Size(tempWidth, tempSize.Height + Math.Abs(AnchorPoint.Y));
                    break;
                case AnchorLocation.TopRight:
                    tempWidth = AnchorPoint.X > AnchorWidth ? tempSize.Width + AnchorPoint.X - AnchorWidth : tempSize.Width;
                    tempSize = new Size(tempWidth, tempSize.Height + Math.Abs(AnchorPoint.Y));
                    break;
                case AnchorLocation.LeftTop:
                    tempHeight = AnchorPoint.Y < 0 ? tempSize.Height + Math.Abs(AnchorPoint.Y) : tempSize.Height;
                    tempSize = new Size(tempSize.Width + Math.Abs(AnchorPoint.X), tempHeight);
                    break;
                case AnchorLocation.LeftBottom:
                    tempHeight = AnchorPoint.Y > AnchorHeight ? tempSize.Height + AnchorPoint.Y - AnchorHeight : tempSize.Height;
                    tempSize = new Size(tempSize.Width + Math.Abs(AnchorPoint.X), tempHeight);
                    break;
                case AnchorLocation.RightTop:
                    tempWidth = tempSize.Width + AnchorPoint.X - AnchorWidth;
                    tempHeight = AnchorPoint.Y < 0 ? tempSize.Height + Math.Abs(AnchorPoint.Y) : tempSize.Height;
                    tempSize = new Size(tempWidth, tempHeight);
                    break;
                case AnchorLocation.RightBottom:
                    tempWidth = tempSize.Width + AnchorPoint.X - AnchorWidth;
                    tempHeight = AnchorPoint.Y > AnchorHeight ? tempSize.Height + AnchorPoint.Y - AnchorHeight : tempSize.Height;
                    tempSize = new Size(tempWidth, tempHeight);
                    break;
                case AnchorLocation.BottomLeft:
                    tempWidth = AnchorPoint.X < 0 ? tempSize.Width + Math.Abs(AnchorPoint.X) : tempSize.Width;
                    tempHeight = tempSize.Height + AnchorPoint.Y - AnchorHeight;
                    tempSize = new Size(tempWidth, tempHeight);
                    break;
                case AnchorLocation.BottomRight:
                    tempWidth = AnchorPoint.X > AnchorWidth ? tempSize.Width + AnchorPoint.X - AnchorWidth : tempSize.Width;
                    tempHeight = tempSize.Height + AnchorPoint.Y - AnchorHeight;
                    tempSize = new Size(tempWidth, tempHeight);
                    break;
            }
            return tempSize;
        }

        #endregion

        protected override Size MeasureOverride(Size constraint)
        {
            UIElement child = Child;
            Size mySize = new Size();

            //If we have a child
            if (child != null)
            {
                // Combine into total decorating size
                Size combined = GetSizeForDecorator(BorderSizeType.CombinedSize);

                // Remove size of border only from child's reference size.
                Size childConstraint = new Size(Math.Max(0.0, constraint.Width - combined.Width),
                                                Math.Max(0.0, constraint.Height - combined.Height));

                child.Measure(childConstraint);
                Size childSize = child.DesiredSize;

                // Now use the returned size to drive our size, by adding back the margins, etc.
                mySize.Width = childSize.Width + combined.Width;
                mySize.Height = childSize.Height + combined.Height;
            }
            else
            {
                // Compute the chrome size added by the various elements
                Size border = HelperCollapseBorder(BorderThickness);
                Size padding = HelperCollapseThickness(Padding);
                // Combine into total decorating size
                mySize = new Size(border.Width + padding.Width, border.Height + padding.Height);
            }

            return mySize;
        }

        #region ArrangeOverride

        private Rect GetChildRect(Size finalSize)
        {
            Rect boundRect = new Rect(finalSize);
            Rect childRect = new Rect(boundRect.Left + BorderThickness + Padding.Left,
                                      boundRect.Top + BorderThickness + Padding.Top,
                                      Math.Max(0.0, AnchorWidth - (BorderThickness * 2 + Padding.Top + Padding.Bottom)),
                                      Math.Max(0.0, AnchorHeight - (BorderThickness * 2 + Padding.Left + Padding.Right)));

            switch (AnchorLocation)
            {
                case AnchorLocation.None:
                case AnchorLocation.RightBottom:
                case AnchorLocation.BottomRight:
                    break;
                case AnchorLocation.TopLeft:
                    childRect.X = AnchorPoint.X < 0 ? childRect.X + Math.Abs(AnchorPoint.X) : childRect.X;
                    childRect.Y = childRect.Y + Math.Abs(AnchorPoint.Y);
                    break;
                case AnchorLocation.TopRight:
                    childRect.Y = childRect.Y + Math.Abs(AnchorPoint.Y);
                    break;
                case AnchorLocation.LeftTop:
                    childRect.X = childRect.X + Math.Abs(AnchorPoint.X);
                    childRect.Y = AnchorPoint.Y < 0 ? childRect.Y + Math.Abs(AnchorPoint.Y) : childRect.Y;
                    break;
                case AnchorLocation.LeftBottom:
                    childRect.X = childRect.X + Math.Abs(AnchorPoint.X);
                    break;
                case AnchorLocation.RightTop:
                    childRect.Y = AnchorPoint.Y < 0 ? childRect.Y + Math.Abs(AnchorPoint.Y) : childRect.Y;
                    break;
                case AnchorLocation.BottomLeft:
                    childRect.X = AnchorPoint.X < 0 ? childRect.X + Math.Abs(AnchorPoint.X) : childRect.X;
                    break;
            }

            return childRect;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            UIElement child = Child;
            if (child != null)
            {
                Rect childRect = GetChildRect(finalSize);
                child.Arrange(childRect);

                Size anchorSize = GetSizeForDecorator(BorderSizeType.CombinedSize);
                Width = anchorSize.Width + childRect.Width;
                Height = anchorSize.Height + childRect.Height;
            }
            else
            {
                Size anchorSize = GetSizeForDecorator(BorderSizeType.OnlyAnchorSize);
                Width = anchorSize.Width + AnchorWidth;
                Height = anchorSize.Height + AnchorHeight;

            }
            finalSize = new Size(Width, Height);
            return finalSize;
        }

        #endregion

        #region OnRender

        protected override void OnRender(DrawingContext dc)
        {
            PaintTail(AnchorLocation, AnchorWidth, AnchorHeight, dc);
            base.OnRender(dc);
        }

        private Point GetDrawStartPoint(AnchorLocation location)
        {
            Point startPoint = new Point(0.0, 0.0);

            switch (AnchorLocation)
            {
                case AnchorLocation.None:
                case AnchorLocation.RightBottom:
                case AnchorLocation.BottomRight:
                    break;
                case AnchorLocation.TopLeft:
                    startPoint.X = AnchorPoint.X < 0 ? startPoint.X + Math.Abs(AnchorPoint.X) : startPoint.X;
                    startPoint.Y = startPoint.Y + Math.Abs(AnchorPoint.Y);
                    break;
                case AnchorLocation.TopRight:
                    startPoint.Y = startPoint.Y + Math.Abs(AnchorPoint.Y);
                    break;
                case AnchorLocation.LeftTop:
                    startPoint.X = startPoint.X + Math.Abs(AnchorPoint.X);
                    startPoint.Y = AnchorPoint.Y < 0 ? startPoint.Y + Math.Abs(AnchorPoint.Y) : startPoint.Y;
                    break;
                case AnchorLocation.LeftBottom:
                    startPoint.X = startPoint.X + Math.Abs(AnchorPoint.X);
                    break;
                case AnchorLocation.RightTop:
                    startPoint.Y = AnchorPoint.Y < 0 ? startPoint.Y + Math.Abs(AnchorPoint.Y) : startPoint.Y;
                    break;
                case AnchorLocation.BottomLeft:
                    startPoint.X = AnchorPoint.X < 0 ? startPoint.X + Math.Abs(AnchorPoint.X) : startPoint.X;
                    break;
            }

            return startPoint;
        }

        private void PaintTail(AnchorLocation location, double W, double H, DrawingContext drawingContext)
        {
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            PathSegmentCollection segmentCollection = new PathSegmentCollection();

            double radius = CornerRadius.BottomLeft;
            Size size = new Size(radius, radius);
            Point startPoint = GetDrawStartPoint(location);

            switch (location)
            {
                case AnchorLocation.None:
                    W = W + startPoint.X;
                    H = H + startPoint.Y;

                    pathFigure.StartPoint = new Point(startPoint.X, radius);
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X, H - radius) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(radius, H), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(W - radius, H) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(W, H - radius), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(W, radius) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(W - radius, startPoint.Y), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(radius, startPoint.Y) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X, radius), Size = size, RotationAngle = 90 });

                    break;

                case AnchorLocation.LeftTop:

                    if (H * 0.15 > radius)
                    {
                        pathFigure.StartPoint = new Point(startPoint.X, radius + startPoint.Y);
                        segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X, startPoint.Y + H * 0.15) });
                    }
                    else
                        pathFigure.StartPoint = new Point(startPoint.X, H * 0.15 + startPoint.Y);
                    segmentCollection.Add(new LineSegment() { Point = AnchorPoint.Y < 0 ? new Point(0, 0) : new Point(0, AnchorPoint.Y + startPoint.Y) }); ///
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X, startPoint.Y + H * 0.35) });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X, startPoint.Y + H - radius) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + radius, startPoint.Y + H), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W - radius, startPoint.Y + H) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W, startPoint.Y + H - radius), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W, startPoint.Y + radius) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W - radius, startPoint.Y), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + radius, startPoint.Y) });
                    if (H * 0.15 > radius)
                    {
                        segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X, startPoint.Y + radius), Size = size, RotationAngle = 90 });
                    }
                    else
                        segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X, startPoint.Y + H * 0.15), Size = size, RotationAngle = 90 });

                    break;

                case AnchorLocation.LeftBottom:

                    pathFigure.StartPoint = new Point(startPoint.X, startPoint.Y + radius);
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X, startPoint.Y + H * 0.65) });
                    segmentCollection.Add(new LineSegment() { Point = new Point(0, AnchorPoint.Y + startPoint.Y) });  ///
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X, startPoint.Y + H * 0.85) });
                    if (H * 0.15 > radius)
                    {
                        segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X, startPoint.Y + H - radius) });
                    }
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + radius, startPoint.Y + H), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W - radius, startPoint.Y + H) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W, startPoint.Y + H - radius), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W, startPoint.Y + radius) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W - radius, startPoint.Y), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + radius, startPoint.Y) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X, startPoint.Y + radius), Size = size, RotationAngle = 90 });

                    break;

                case AnchorLocation.TopLeft:

                    pathFigure.StartPoint = new Point(startPoint.X, startPoint.Y + radius);
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X, startPoint.Y + H - radius) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + radius, startPoint.Y + H), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W - radius, startPoint.Y + H) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W, startPoint.Y + H - radius), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W, startPoint.Y + radius) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W - radius, startPoint.Y), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W * 0.35, startPoint.Y) });
                    segmentCollection.Add(new LineSegment() { Point = AnchorPoint.X < 0 ? new Point(0.0, 0.0) : new Point(AnchorPoint.X + startPoint.X, 0) });///
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W * 0.15, startPoint.Y) });
                    if (W * 0.15 > radius)
                        segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + radius, startPoint.Y) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X, startPoint.Y + radius), Size = size, RotationAngle = 90 });

                    break;

                case AnchorLocation.TopRight:

                    pathFigure.StartPoint = new Point(startPoint.X, startPoint.Y + radius);
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X, startPoint.Y + H - radius) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + radius, startPoint.Y + H), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W - radius, startPoint.Y + H) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W, startPoint.Y + H - radius), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W, startPoint.Y + radius) });
                    if (W * 0.15 > radius)
                    {
                        segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W - radius, startPoint.Y), Size = size, RotationAngle = 90 });
                        segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W * 0.85, startPoint.Y) });
                    }
                    else
                        segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W * 0.85, startPoint.Y), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(AnchorPoint.X + startPoint.X, 0) });///
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W * 0.65, startPoint.Y) });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + radius, startPoint.Y) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X, startPoint.Y + radius), Size = size, RotationAngle = 90 });

                    break;

                case AnchorLocation.RightTop:

                    pathFigure.StartPoint = new Point(startPoint.X, startPoint.Y + radius);
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X, startPoint.Y + H - radius) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + radius, startPoint.Y + H), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W - radius, startPoint.Y + H) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W, startPoint.Y + H - radius), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W, startPoint.Y + H * 0.35) });
                    segmentCollection.Add(new LineSegment() { Point = AnchorPoint.Y < 0 ? new Point(AnchorPoint.X + startPoint.X, 0) : new Point(AnchorPoint.X + startPoint.X, AnchorPoint.Y + startPoint.Y) });///
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W, startPoint.Y + H * 0.15) });
                    if (H * 0.15 > radius)
                        segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W, startPoint.Y + radius) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W - radius, startPoint.Y), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + radius, startPoint.Y) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X, startPoint.Y + radius), Size = size, RotationAngle = 90 });

                    break;

                case AnchorLocation.RightBottom:

                    pathFigure.StartPoint = new Point(startPoint.X, startPoint.Y + radius);
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X, startPoint.Y + H - radius) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + radius, startPoint.Y + H), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W - radius, startPoint.Y + H) });
                    if (H * 0.15 > radius)
                    {
                        segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W, startPoint.Y + H - radius), Size = size, RotationAngle = 90 });
                        segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W, startPoint.Y + H * 0.85) });
                    }
                    else
                        segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W, startPoint.Y + H * 0.85), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W, startPoint.Y + H * 0.85) });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + AnchorPoint.X, AnchorPoint.Y + startPoint.Y) });///
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W, startPoint.Y + H * 0.65) });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W, startPoint.Y + radius) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W - radius, startPoint.Y), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + radius, startPoint.Y) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X, startPoint.Y + radius), Size = size, RotationAngle = 90 });

                    break;

                case AnchorLocation.BottomLeft:

                    pathFigure.StartPoint = new Point(startPoint.X, startPoint.Y + radius);
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X, startPoint.Y + H - radius) });
                    if (W * 0.15 > radius)
                    {
                        segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + radius, startPoint.Y + H), Size = size, RotationAngle = 90 });
                        segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W * 0.15, startPoint.Y + H) });
                    }
                    else
                        segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W * 0.15, startPoint.Y + H), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = AnchorPoint.X < 0 ? new Point(0, AnchorPoint.Y + startPoint.Y) : new Point(AnchorPoint.X + startPoint.X, AnchorPoint.Y + startPoint.Y) });///
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W * 0.35, startPoint.Y + H) });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W - radius, startPoint.Y + H) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W, startPoint.Y + H - radius), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W, startPoint.Y + radius) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W - radius, startPoint.Y), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + radius, startPoint.Y) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X, startPoint.Y + radius), Size = size, RotationAngle = 90 });

                    break;

                case AnchorLocation.BottomRight:

                    pathFigure.StartPoint = new Point(startPoint.X, startPoint.Y + radius);
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X, startPoint.Y + H - radius) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + radius, startPoint.Y + H), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W * 0.65, startPoint.Y + H) });
                    segmentCollection.Add(new LineSegment() { Point = new Point(AnchorPoint.X + startPoint.X, AnchorPoint.Y + startPoint.Y) });///
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W * 0.85, startPoint.Y + H) });
                    if (W * 0.15 > radius)
                        segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W - radius, startPoint.Y + H) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W, startPoint.Y + H - radius), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + W, startPoint.Y + radius) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X + W - radius, startPoint.Y), Size = size, RotationAngle = 90 });
                    segmentCollection.Add(new LineSegment() { Point = new Point(startPoint.X + radius, startPoint.Y) });
                    segmentCollection.Add(new ArcSegment() { Point = new Point(startPoint.X, startPoint.Y + radius), Size = size, RotationAngle = 90 });

                    break;
            }

            pathFigure.Segments = segmentCollection;
            pathGeometry.Figures = new PathFigureCollection() { pathFigure };

            drawingContext.DrawGeometry(Background, new Pen(BorderBrush, BorderThickness), pathGeometry);
        }

        #endregion

    }
}
