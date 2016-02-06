using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.ComponentModel;

//http://www.codeproject.com/Articles/38361/Circular-gauge-custom-control-for-Silverlight-an
namespace MyControl.Resources.Controls
{
    [TemplatePart(Name = PART_LayoutRoot, Type = typeof(Grid))]
    [TemplatePart(Name = PART_IndicatorRoot, Type = typeof(Grid))]
    [TemplatePart(Name = PART_PointerCap, Type = typeof(Ellipse))]
    [TemplatePart(Name = PART_Pointer, Type = typeof(Path))]
    [TemplatePart(Name = PART_PointerRT, Type = typeof(RotateTransform))]
    internal class RadialGuage : Control
    {
        private const string PART_LayoutRoot = "PART_LayoutRoot";
        private const string PART_IndicatorRoot = "PART_IndicatorRoot";
        private const string PART_Pointer = "PART_Pointer";
        private const string PART_PointerCap = "PART_PointerCap";
        private const string PART_PointerRT = "PART_PointerRT";

        private Grid LayoutRoot = null;
        private Grid IndicatorRoot = null;
        private Ellipse PointerCap = null;
        private Path Pointer = null;
        private RotateTransform PointerRT = null;

        static RadialGuage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadialGuage), new FrameworkPropertyMetadata(typeof(RadialGuage)));
        }

        public static readonly DependencyProperty TitleMarginProperty = DependencyProperty.Register("TitleMargin", typeof(Thickness), typeof(RadialGuage), new FrameworkPropertyMetadata(new Thickness()));
        public Thickness TitleMargin
        {
            get { return (Thickness)GetValue(TitleMarginProperty); }
            set { SetValue(TitleMarginProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(RadialGuage));
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(RadialGuage), new UIPropertyMetadata(90d, PropertyChangedCallback));
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty ScaleRadiusProperty = DependencyProperty.Register("ScaleRadius", typeof(double), typeof(RadialGuage), new UIPropertyMetadata(50d, PropertyChangedCallback));
        public double ScaleRadius
        {
            get { return (double)GetValue(ScaleRadiusProperty); }
            set { SetValue(ScaleRadiusProperty, value); }
        }

        public static readonly DependencyProperty ScaleTextRadiusProperty = DependencyProperty.Register("ScaleTextRadius", typeof(double), typeof(RadialGuage), new UIPropertyMetadata(70d, PropertyChangedCallback));
        public double ScaleTextRadius
        {
            get { return (double)GetValue(ScaleTextRadiusProperty); }
            set { SetValue(ScaleTextRadiusProperty, value); }
        }

        public static readonly DependencyProperty IndicatorRadiusProperty = DependencyProperty.Register("IndicatorRadius", typeof(double), typeof(RadialGuage), new UIPropertyMetadata(35d, PropertyChangedCallback));
        public double IndicatorRadius
        {
            get { return (double)GetValue(IndicatorRadiusProperty); }
            set { SetValue(IndicatorRadiusProperty, value); }
        }

        public static readonly DependencyProperty IndicatorThicknessProperty = DependencyProperty.Register("IndicatorThickness", typeof(double), typeof(RadialGuage), new UIPropertyMetadata(8d, PropertyChangedCallback));
        public double IndicatorThickness
        {
            get { return (double)GetValue(IndicatorThicknessProperty); }
            set { SetValue(IndicatorThicknessProperty, value); }
        }

        public static readonly DependencyProperty IsShowIndicatorProperty = DependencyProperty.Register("IsShowIndicator", typeof(bool), typeof(RadialGuage), new UIPropertyMetadata(true, PropertyChangedCallback));
        public bool IsShowIndicator
        {
            get { return (bool)GetValue(IsShowIndicatorProperty); }
            set { SetValue(IsShowIndicatorProperty, value); }
        }

        public static readonly DependencyProperty PointeCapDiameterProperty = DependencyProperty.Register("PointeCapDiameter", typeof(double), typeof(RadialGuage), new UIPropertyMetadata(25d, PropertyChangedCallback));
        public double PointeCapDiameter
        {
            get { return (double)GetValue(PointeCapDiameterProperty); }
            set { SetValue(PointeCapDiameterProperty, value); }
        }

        public static readonly DependencyProperty ScaleFontSizeProperty = DependencyProperty.Register("ScaleFontSize", typeof(double), typeof(RadialGuage), new UIPropertyMetadata(10d, PropertyChangedCallback));
        public double ScaleFontSize
        {
            get { return (double)GetValue(ScaleFontSizeProperty); }
            set { SetValue(ScaleFontSizeProperty, value); }
        }

        public static readonly DependencyProperty MinScaleProperty = DependencyProperty.Register("MinScale", typeof(double), typeof(RadialGuage), new FrameworkPropertyMetadata(0.0, PropertyChangedCallback));
        public double MinScale
        {
            get { return (double)GetValue(MinScaleProperty); }
            set { SetValue(MinScaleProperty, value); }
        }

        public static readonly DependencyProperty MaxScaleProperty = DependencyProperty.Register("MaxScale", typeof(double), typeof(RadialGuage), new FrameworkPropertyMetadata(60d, PropertyChangedCallback));
        public double MaxScale
        {
            get { return (double)GetValue(MaxScaleProperty); }
            set { SetValue(MaxScaleProperty, value); }
        }

        public static readonly DependencyProperty MajorScaleCountProperty = DependencyProperty.Register("MajorScaleCount", typeof(int), typeof(RadialGuage), new UIPropertyMetadata(6, PropertyChangedCallback));
        public int MajorScaleCount
        {
            get { return (int)GetValue(MajorScaleCountProperty); }
            set { SetValue(MajorScaleCountProperty, value); }
        }

        public static readonly DependencyProperty MinorScaleCountProperty = DependencyProperty.Register("MinorScaleCount", typeof(int), typeof(RadialGuage), new UIPropertyMetadata(5, PropertyChangedCallback));
        public int MinorScaleCount
        {
            get { return (int)GetValue(MinorScaleCountProperty); }
            set { SetValue(MinorScaleCountProperty, value); }
        }

        public static readonly DependencyProperty ScaleStartAngleProperty = DependencyProperty.Register("ScaleStartAngle", typeof(double), typeof(RadialGuage), new UIPropertyMetadata(120d, PropertyChangedCallback));
        public double ScaleStartAngle
        {
            get { return (double)GetValue(ScaleStartAngleProperty); }
            set { SetValue(ScaleStartAngleProperty, value); }
        }

        public static readonly DependencyProperty ScaleSweepAngleProperty = DependencyProperty.Register("ScaleSweepAngle", typeof(double), typeof(RadialGuage), new UIPropertyMetadata(300d, PropertyChangedCallback));
        public double ScaleSweepAngle
        {
            get { return (double)GetValue(ScaleSweepAngleProperty); }
            set { SetValue(ScaleSweepAngleProperty, value); }
        }

        public static readonly DependencyProperty IndicatorOptimalStartScaleProperty = DependencyProperty.Register("IndicatorOptimalStartScale", typeof(double), typeof(RadialGuage), new UIPropertyMetadata(20d, PropertyChangedCallback, IndicatorOptimalValuePropertyCoerceValueCallback));
        public double IndicatorOptimalStartScale
        {
            get { return (double)GetValue(IndicatorOptimalStartScaleProperty); }
            set { SetValue(IndicatorOptimalStartScaleProperty, value); }
        }

        static object IndicatorOptimalValuePropertyCoerceValueCallback(DependencyObject d, object value)
        {
            RadialGuage obj = d as RadialGuage;
            double CurValue = (double)value;

            if (CurValue < obj.MinScale)
                return obj.MinScale;
            if (CurValue > obj.MaxScale)
                return obj.MaxScale;

            return CurValue;
        }

        public static readonly DependencyProperty IndicatorOptimalEndScaleProperty = DependencyProperty.Register("IndicatorOptimalEndScale", typeof(double), typeof(RadialGuage), new UIPropertyMetadata(50d, PropertyChangedCallback, IndicatorOptimalValuePropertyCoerceValueCallback));
        public double IndicatorOptimalEndScale
        {
            get { return (double)GetValue(IndicatorOptimalEndScaleProperty); }
            set { SetValue(IndicatorOptimalEndScaleProperty, value); }
        }

        public static readonly DependencyProperty PointerLengthProperty = DependencyProperty.Register("PointerLength", typeof(double), typeof(RadialGuage), new UIPropertyMetadata(50d, PropertyChangedCallback));
        public double PointerLength
        {
            get { return (double)GetValue(PointerLengthProperty); }
            set { SetValue(PointerLengthProperty, value); }
        }

        public static readonly DependencyProperty CurrentScaleProperty = DependencyProperty.Register("CurrentScale", typeof(double), typeof(RadialGuage), new FrameworkPropertyMetadata(20d, CurrentScalePropertyChangedCallback, CurrentScalePropertyCoerceValueCallback));
        public double CurrentScale
        {
            get { return (double)GetValue(CurrentScaleProperty); }
            set { SetValue(CurrentScaleProperty, value); }
        }

        static void CurrentScalePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadialGuage obj = d as RadialGuage;
            double newValue = (double)e.NewValue;
            double oldValue = (double)e.OldValue;

            if (oldValue != newValue)
                obj.MovePointerBetweenScaleWithAnimation(oldValue, newValue);
        }
        static object CurrentScalePropertyCoerceValueCallback(DependencyObject d, object value)
        {
            RadialGuage obj = d as RadialGuage;

            if ((double)value < obj.MinScale)
                return obj.MinScale;

            if ((double)value > obj.MaxScale)
                return obj.MaxScale;

            return value;
        }

        static void PropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadialGuage obj = sender as RadialGuage;
            obj.DrawUIElements();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            LayoutRoot = GetTemplateChild(PART_LayoutRoot) as Grid;
            IndicatorRoot = GetTemplateChild(PART_IndicatorRoot) as Grid;
            PointerCap = GetTemplateChild(PART_PointerCap) as Ellipse;
            Pointer = GetTemplateChild(PART_Pointer) as Path;
            PointerRT = GetTemplateChild(PART_PointerRT) as RotateTransform;

            DrawUIElements();

            if (IsShowIndicator)
            {
                var v = DependencyPropertyDescriptor.FromProperty(RotateTransform.AngleProperty, typeof(RotateTransform));
                v.AddValueChanged(PointerRT, OnAnglePropertyChanged);
            }
        }

        private void OnAnglePropertyChanged(object sender, EventArgs e)
        {
            RotateTransform obj = sender as RotateTransform;
            DrawIndicatorByScale(GetScaleByAngle(obj.Angle));
        }

        private void DrawUIElements()
        {
            if (LayoutRoot == null)
                return;

            LayoutRoot.Children.Clear();

            //画刻度
            DrawScale();
            //指针
            MovePointerToScale(CurrentScale);
            //指示器
            //DrawIndicator();
            DrawIndicatorByScale(CurrentScale);

        }

        //创建刻度线控件
        private Rectangle GetScaleRect(double _Width, double _Height, Brush _Fill)
        {
            Rectangle ScaleRect = new Rectangle();
            ScaleRect.Height = _Height;
            ScaleRect.Width = _Width;
            ScaleRect.Fill = _Fill;
            ScaleRect.RenderTransformOrigin = new Point(0.5, 0.5);
            ScaleRect.HorizontalAlignment = HorizontalAlignment.Center;
            ScaleRect.VerticalAlignment = VerticalAlignment.Center;

            return ScaleRect;
        }

        //创建刻度值控件
        private TextBlock GetScaleText(string _Text, Brush _Fg)
        {
            TextBlock ScaleText = new TextBlock();

            ScaleText.Text = _Text;
            ScaleText.FontSize = ScaleFontSize;
            ScaleText.Foreground = _Fg;
            //ScaleText.RenderTransformOrigin = new Point(0.5, 0.5);    //刻度值不需要旋转
            ScaleText.TextAlignment = TextAlignment.Center;
            ScaleText.VerticalAlignment = VerticalAlignment.Center;
            ScaleText.HorizontalAlignment = HorizontalAlignment.Center;

            return ScaleText;
        }

        /// <summary>
        /// 计算刻度线及刻度值的旋转或偏移位置
        /// </summary> 
        /// <param name="_UIElementAngle">当前元素所在角度</param>
        /// <param name="_UIElementRadius">当前元素所在半径</param>
        /// <param name="_IsNeedRotate">是否需要旋转</param>
        private TransformGroup GetUIElementTransform(double _UIElementAngle, double _UIElementRadius, bool _IsNeedRotate)
        {
            TransformGroup transformGroup = new TransformGroup();

            if (_IsNeedRotate)
            {
                //计算旋转角度
                RotateTransform rotateTransform = new RotateTransform();
                rotateTransform.Angle = _UIElementAngle;

                transformGroup.Children.Add(rotateTransform);
            }

            //计算偏移
            TranslateTransform translateTransform = new TranslateTransform();
            Point pos = GetSpecifiedPosByAngle(_UIElementAngle, _UIElementRadius, true);
            translateTransform.X = pos.X;
            translateTransform.Y = pos.Y;

            transformGroup.Children.Add(translateTransform);

            return transformGroup;
        }

        private void DrawScale()
        {
            if (LayoutRoot == null || PointerCap == null || Pointer == null)
                return;

            //画主刻度
            for (int MajorScaleIndex = 0; MajorScaleIndex <= MajorScaleCount; MajorScaleIndex++)
            {
                //刻度线
                Rectangle MajorScaleRect = GetScaleRect(10, 3, Foreground);
                //刻度值
                double ScaleText = MajorScaleIndex == MajorScaleCount ? MaxScale : MinScale + MajorScaleIndex * Math.Round((MaxScale - MinScale) / MajorScaleCount);
                TextBlock ScaleTextBlock = GetScaleText(ScaleText.ToString(), Foreground);

                //计算位置
                double MajorScaleAngle = MajorScaleIndex == MajorScaleCount ? ScaleStartAngle + ScaleSweepAngle : ScaleStartAngle + MajorScaleIndex * (ScaleSweepAngle / MajorScaleCount);
                MajorScaleRect.RenderTransform = GetUIElementTransform(MajorScaleAngle, ScaleRadius, true);
                ScaleTextBlock.RenderTransform = GetUIElementTransform(MajorScaleAngle, ScaleTextRadius, false);

                LayoutRoot.Children.Insert(0, MajorScaleRect);
                LayoutRoot.Children.Insert(0, ScaleTextBlock);

                //最后一个主刻度后面没有次刻度线
                if (MajorScaleIndex == MajorScaleCount)
                    break;

                //画次刻度线
                for (int MinorScaleIndex = 1; MinorScaleIndex < MinorScaleCount; MinorScaleIndex++)
                {
                    //创建刻度控件
                    Rectangle MinorScaleRect = GetScaleRect(3, 1, Brushes.White);

                    //计算位置
                    double MinorScaleAngle = MajorScaleAngle + MinorScaleIndex * (ScaleSweepAngle / MajorScaleCount / MinorScaleCount);
                    MinorScaleRect.RenderTransform = GetUIElementTransform(MinorScaleAngle, ScaleRadius, true);

                    LayoutRoot.Children.Insert(0, MinorScaleRect);
                }
            }

            LayoutRoot.Children.Add(Pointer);
            LayoutRoot.Children.Add(PointerCap);
        }

        //移动到指定角度
        private void MovePointerToAngle(double angleValue)
        {
            if (Pointer != null)
            {
                TransformGroup transformGroup = Pointer.RenderTransform as TransformGroup;
                RotateTransform rotateTransform = transformGroup.Children[0] as RotateTransform;
                rotateTransform.Angle = angleValue;
            }
        }

        //移动到指定刻度
        private void MovePointerToScale(double scaleValue)
        {
            if (Pointer != null)
            {
                MovePointerToAngle(GetAngleByScale(scaleValue));
            }
        }

        //刻度之间的移动
        private void MovePointerBetweenScaleWithAnimation(double _OldScale, double _NewScale)
        {
            if (Pointer != null && _OldScale != _NewScale)
            {
                MovePointerBetweenAngleWithAnimation(GetAngleByScale(_OldScale), GetAngleByScale(_NewScale));
            }
        }

        //角度之间的移动
        private void MovePointerBetweenAngleWithAnimation(double _OldAngle, double _NewAngle)
        {
            if (Pointer != null && _OldAngle != _NewAngle)
            {
                DoubleAnimation da = new DoubleAnimation();
                da.From = _OldAngle;
                da.To = _NewAngle;

                double animDuration = Math.Abs(_OldAngle - _NewAngle) * 6;
                da.Duration = new Duration(TimeSpan.FromMilliseconds(animDuration));

                Storyboard sb = new Storyboard();
                sb.Completed += new EventHandler(sb_Completed);
                sb.Children.Add(da);
                Storyboard.SetTarget(da, Pointer);
                Storyboard.SetTargetProperty(da, new PropertyPath("(Path.RenderTransform).(TransformGroup.Children)[0].(RotateTransform.Angle)"));

                sb.Begin();
            }
        }

        void sb_Completed(object sender, EventArgs e)
        {
        }

        private void DrawIndicatorByScale(double _CurrentScale)
        {
            if (IsShowIndicator == false)
                return;

            IndicatorRoot.Children.Clear();

            Point BelowRangeStartOutPoint = GetSpecifiedPosByAngle(ScaleStartAngle, IndicatorRadius + IndicatorThickness, false);
            Point BelowRangeStartInPoint = GetSpecifiedPosByAngle(ScaleStartAngle, IndicatorRadius, false);

            Point CurrentScaleOutPoint = GetSpecifiedPosByScale(_CurrentScale, IndicatorRadius + IndicatorThickness, false);
            Point CurrentScaleInPoint = GetSpecifiedPosByScale(_CurrentScale, IndicatorRadius, false);

            //值处于第一段的范围内，则只画起始值到当前值的弧度
            if (_CurrentScale <= IndicatorOptimalStartScale)
            {
                bool IsLargeArcCurrent = GetAngleByScale(_CurrentScale) - ScaleStartAngle > 180d;
                DrawIndicatorSegment(BelowRangeStartOutPoint, CurrentScaleOutPoint, CurrentScaleInPoint, BelowRangeStartInPoint, IsLargeArcCurrent, Brushes.Yellow);

                return;
            }

            Point OptimalRangeStartOutPoint = GetSpecifiedPosByScale(IndicatorOptimalStartScale, IndicatorRadius + IndicatorThickness, false);
            Point OptimalRangeStartInPoint = GetSpecifiedPosByScale(IndicatorOptimalStartScale, IndicatorRadius, false);

            //画第一段
            bool IsLargeArcOne = GetAngleByScale(IndicatorOptimalStartScale) - ScaleStartAngle > 180d;
            DrawIndicatorSegment(BelowRangeStartOutPoint, OptimalRangeStartOutPoint, OptimalRangeStartInPoint, BelowRangeStartInPoint, IsLargeArcOne, Brushes.Yellow);

            //值处于第二段的范围内，则画第一段及第二段起始值到当前值的弧度
            if (_CurrentScale <= IndicatorOptimalEndScale)
            {
                //画第二段
                bool IsLargeArcCurrent = GetAngleByScale(_CurrentScale) - GetAngleByScale(IndicatorOptimalStartScale) > 180d;
                DrawIndicatorSegment(OptimalRangeStartOutPoint, CurrentScaleOutPoint, CurrentScaleInPoint, OptimalRangeStartInPoint, IsLargeArcCurrent, Brushes.Green);

                return;
            }

            //值处于第三段的范围内，则画第一段，第二段及第三段起始值到当前值的弧度
            Point AboveRangeStartOutPoint = GetSpecifiedPosByScale(IndicatorOptimalEndScale, IndicatorRadius + IndicatorThickness, false);
            Point AboveRangeStartInPoint = GetSpecifiedPosByScale(IndicatorOptimalEndScale, IndicatorRadius, false);
            bool IsLargeArcTwo = GetAngleByScale(IndicatorOptimalEndScale) - GetAngleByScale(IndicatorOptimalStartScale) > 180d;

            //画第二段
            DrawIndicatorSegment(OptimalRangeStartOutPoint, AboveRangeStartOutPoint, AboveRangeStartInPoint, OptimalRangeStartInPoint, IsLargeArcTwo, Brushes.Green);

            //画第三段
            bool IsLargeArcThree = GetAngleByScale(_CurrentScale) - GetAngleByScale(IndicatorOptimalEndScale) > 180d;
            DrawIndicatorSegment(AboveRangeStartOutPoint, CurrentScaleOutPoint, CurrentScaleInPoint, AboveRangeStartInPoint, IsLargeArcThree, Brushes.Red);
        }

        private void DrawIndicator()
        {
            if (IsShowIndicator == false)
                return;

            IndicatorRoot.Children.Clear();

            Point BelowRangeStartOutPoint = GetSpecifiedPosByAngle(ScaleStartAngle, IndicatorRadius + IndicatorThickness, false);
            Point BelowRangeStartInPoint = GetSpecifiedPosByAngle(ScaleStartAngle, IndicatorRadius, false);

            Point OptimalRangeStartOutPoint = GetSpecifiedPosByScale(IndicatorOptimalStartScale, IndicatorRadius + IndicatorThickness, false);
            Point OptimalRangeStartInPoint = GetSpecifiedPosByScale(IndicatorOptimalStartScale, IndicatorRadius, false);
            bool IsLargeArcOne = GetAngleByScale(IndicatorOptimalStartScale) - ScaleStartAngle > 180d;

            //画第一段
            DrawIndicatorSegment(BelowRangeStartOutPoint, OptimalRangeStartOutPoint, OptimalRangeStartInPoint, BelowRangeStartInPoint, IsLargeArcOne, Brushes.Yellow);

            Point AboveRangeStartOutPoint = GetSpecifiedPosByScale(IndicatorOptimalEndScale, IndicatorRadius + IndicatorThickness, false);
            Point AboveRangeStartInPoint = GetSpecifiedPosByScale(IndicatorOptimalEndScale, IndicatorRadius, false);
            bool IsLargeArcTwo = GetAngleByScale(IndicatorOptimalEndScale) - GetAngleByScale(IndicatorOptimalStartScale) > 180d;

            //画第二段
            DrawIndicatorSegment(OptimalRangeStartOutPoint, AboveRangeStartOutPoint, AboveRangeStartInPoint, OptimalRangeStartInPoint, IsLargeArcTwo, Brushes.Green);

            Point AboveRangeEndOutPoint = GetSpecifiedPosByAngle(ScaleStartAngle + ScaleSweepAngle, IndicatorRadius + IndicatorThickness, false);
            Point AboveRangeEndInPoint = GetSpecifiedPosByAngle(ScaleStartAngle + ScaleSweepAngle, IndicatorRadius, false);
            bool IsLargeArcThree = ScaleStartAngle + ScaleSweepAngle - GetAngleByScale(IndicatorOptimalEndScale) > 180d;

            //画第三段
            DrawIndicatorSegment(AboveRangeStartOutPoint, AboveRangeEndOutPoint, AboveRangeEndInPoint, AboveRangeStartInPoint, IsLargeArcThree, Brushes.Red);
        }

        private void DrawIndicatorSegment(Point _Out1Point, Point _Out2Point, Point _In2Point, Point _In1Point, bool _IsLargeArc, Brush _Fill)
        {
            if (LayoutRoot == null)
                return;

            double OutRadius = IndicatorRadius + IndicatorThickness;
            double InRadius = IndicatorRadius;

            PathSegmentCollection segmentCollection = new PathSegmentCollection();

            PathFigure pathFigure = new PathFigure()
            {
                //确定起点（外弧线左侧点）
                StartPoint = _Out1Point,
                Segments = segmentCollection,
                IsClosed = true
            };

            //连到外弧线右侧点
            segmentCollection.Add(new ArcSegment()
            {
                Size = new Size(OutRadius, OutRadius),
                Point = _Out2Point,
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = _IsLargeArc
            });

            //连到内弧线右侧点
            segmentCollection.Add(new LineSegment() { Point = _In2Point });

            //连到内弧线左侧点
            segmentCollection.Add(new ArcSegment()
            {
                Size = new Size(InRadius, InRadius),
                Point = _In1Point,
                SweepDirection = SweepDirection.Counterclockwise,
                IsLargeArc = _IsLargeArc
            });

            Path IndicatorPath = new Path()
            {
                RenderTransformOrigin = new Point(0.5, 0.5),
                Fill = _Fill,
                Data = new PathGeometry()
                {
                    Figures = new PathFigureCollection() { pathFigure }
                }
            };

            IndicatorRoot.Children.Insert(0, IndicatorPath);

        }

        /// <summary>
        /// 根据刻度值获取角度值
        /// </summary>
        /// <param name="_Scale">刻度值</param>
        private double GetAngleByScale(double _Scale)
        {
            double anglePerScale = ScaleSweepAngle / (MaxScale - MinScale);

            double moveScaleValue = Math.Abs(MinScale) + _Scale;
            double moveAngleValue = ((double)(moveScaleValue * anglePerScale));

            return moveAngleValue + ScaleStartAngle;
        }

        private double GetScaleByAngle(double _Angle)
        {
            double scalePerAngle = (MaxScale - MinScale) / ScaleSweepAngle;

            double moveAngleValue = _Angle - ScaleStartAngle;
            double moveScaleValue = moveAngleValue * scalePerAngle;

            return MinScale + moveScaleValue;
        }

        /// <summary>
        /// 根据刻度值获取指定位置
        /// </summary>
        /// <param name="_PosScale">刻度</param>
        /// <param name="_PosRadius">半径</param>
        /// <param name="IsRelativeCenter">是否相对于中心点，否则相对于左上角</param>
        private Point GetSpecifiedPosByScale(double _PosScale, double _PosRadius, bool IsRelativeCenter)
        {
            return GetSpecifiedPosByAngle(GetAngleByScale(_PosScale), _PosRadius, IsRelativeCenter);
        }

        /// <summary>
        /// 根据角度值获取指定位置
        /// </summary>
        /// <param name="_PosAngle">角度</param>
        /// <param name="_PosRadius">半径</param>
        /// <param name="IsRelativeCenter">是否相对于中心点，否则相对于左上角</param>
        private Point GetSpecifiedPosByAngle(double _PosAngle, double _PosRadius, bool IsRelativeCenter)
        {
            double AngleRadian = (_PosAngle * Math.PI) / 180;   //计算得到该角度的弧度
            Point Pos = new Point(_PosRadius * Math.Cos(AngleRadian), _PosRadius * Math.Sin(AngleRadian));

            if (!IsRelativeCenter)
            {
                Pos.X += Radius;
                Pos.Y += Radius;
            }

            return Pos;
        }
    }
}
