using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyControl.Resources.Controls
{
    class PathCheckBox : CheckBox
    {
        public PathCheckBox()
        {
            DefaultStyleKey = typeof(PathCheckBox);
        } 

        #region Share

        public static readonly DependencyProperty IsBgTransparentProperty = DependencyProperty.Register("IsBgTransparent", typeof(bool), typeof(PathCheckBox),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
        public bool IsBgTransparent
        {
            get { return (bool)GetValue(IsBgTransparentProperty); }
            set { SetValue(IsBgTransparentProperty, value); }
        }

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register( "Stretch", typeof(Stretch), typeof(PathCheckBox),              
            new FrameworkPropertyMetadata(Stretch.None, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange)); 
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public new static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(PathCheckBox),
            new FrameworkPropertyMetadata(new Thickness(0,0,0,0), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
        public new Thickness BorderThickness 
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public new static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(PathCheckBox),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
        public new Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(PathCheckBox),
            new FrameworkPropertyMetadata(new CornerRadius(0,0,0,0), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }  

        #endregion

        #region CheckedData

        public static readonly DependencyProperty CheckedDataProperty = DependencyProperty.Register("CheckedData", typeof(Geometry), typeof(PathCheckBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender), null);
        public Geometry CheckedData
        {
            get { return (Geometry)GetValue(CheckedDataProperty); }
            set { SetValue(CheckedDataProperty, value); }
        }

        public static readonly DependencyProperty CheckedToolTipProperty = DependencyProperty.Register("CheckedToolTip", typeof(string), typeof(PathCheckBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender), null);
        public string CheckedToolTip
        {
            get { return (string)GetValue(CheckedToolTipProperty); }
            set { SetValue(CheckedToolTipProperty, value); }
        } 

        public static readonly DependencyProperty CheckedFillProperty = DependencyProperty.Register("CheckedFill", typeof(Brush), typeof(PathCheckBox),
                        new FrameworkPropertyMetadata((Brush)null,FrameworkPropertyMetadataOptions.AffectsRender |FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
        public Brush CheckedFill
        {
            get { return (Brush)GetValue(CheckedFillProperty); }
            set { SetValue(CheckedFillProperty, value); }
        }

        public static readonly DependencyProperty CheckedDataWidthProperty = DependencyProperty.Register("CheckedDataWidth", typeof(double), typeof(PathCheckBox),
            new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender), null);
        public double CheckedDataWidth
        {
            get { return (double)GetValue(CheckedDataWidthProperty); }
            set { SetValue(CheckedDataWidthProperty, value); }
        }

        public static readonly DependencyProperty CheckedDataHeightProperty = DependencyProperty.Register("CheckedDataHeight", typeof(double), typeof(PathCheckBox),
            new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender), null);
        public double CheckedDataHeight
        {
            get { return (double)GetValue(CheckedDataHeightProperty); }
            set { SetValue(CheckedDataHeightProperty, value); }
        } 

        #endregion

        #region UnCheckedData

        public static readonly DependencyProperty UnCheckedDataProperty = DependencyProperty.Register("UnCheckedData", typeof(Geometry), typeof(PathCheckBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender), null);
        public Geometry UnCheckedData
        {
            get { return (Geometry)GetValue(UnCheckedDataProperty); }
            set { SetValue(UnCheckedDataProperty, value); }
        }

        public static readonly DependencyProperty UnCheckedToolTipProperty = DependencyProperty.Register("UnCheckedToolTip", typeof(string), typeof(PathCheckBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender), null);
        public string UnCheckedToolTip
        {
            get { return (string)GetValue(UnCheckedToolTipProperty); }
            set { SetValue(UnCheckedToolTipProperty, value); }
        } 

        public static readonly DependencyProperty UnCheckedFillProperty = DependencyProperty.Register("UnCheckedFill", typeof(Brush), typeof(PathCheckBox),
                        new FrameworkPropertyMetadata((Brush)null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
        public Brush UnCheckedFill
        {
            get { return (Brush)GetValue(UnCheckedFillProperty); }
            set { SetValue(UnCheckedFillProperty, value); }
        }

        public static readonly DependencyProperty UnCheckedDataWidthProperty = DependencyProperty.Register("UnCheckedDataWidth", typeof(double), typeof(PathCheckBox),
            new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender), null);
        public double UnCheckedDataWidth
        {
            get { return (double)GetValue(UnCheckedDataWidthProperty); }
            set { SetValue(UnCheckedDataWidthProperty, value); }
        }

        public static readonly DependencyProperty UnCheckedDataHeightProperty = DependencyProperty.Register("UnCheckedDataHeight", typeof(double), typeof(PathCheckBox),
            new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender), null);
        public double UnCheckedDataHeight
        {
            get { return (double)GetValue(UnCheckedDataHeightProperty); }
            set { SetValue(UnCheckedDataHeightProperty, value); }
        } 

        #endregion
    }
}
