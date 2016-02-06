using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace MyControl.Resources.Controls
{
    public enum MatrixType
    {
        SingleRow,
        SingleColumn,
        Auto
    }

    public class MatrixPanel : Panel
    {
        int Columns = 0;
        int Rows = 0;
        int RealChildCount = 0;
        Size SpaceSize = new Size();

        public MatrixPanel()
        {
        }

        public static readonly DependencyProperty SpaceProperty = DependencyProperty.Register("Space", typeof(double), typeof(MatrixPanel), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
        public double Space
        {
            get { return (double)GetValue(SpaceProperty); }
            set { SetValue(SpaceProperty, value); }
        }

        public static readonly DependencyProperty MatrixTypeProperty = DependencyProperty.Register("MatrixType", typeof(MatrixType), typeof(MatrixPanel), new FrameworkPropertyMetadata(MatrixType.Auto, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
        public MatrixType MatrixType
        {
            get { return (MatrixType)GetValue(MatrixTypeProperty); }
            set { SetValue(MatrixTypeProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            ComputedColumnAndRow(MatrixType, out RealChildCount, out Columns, out Rows);

            if (RealChildCount == 0)
                return base.MeasureOverride(availableSize);

            SpaceSize = GetSpaceSize(RealChildCount);

            double maxChildDesiredWidth = 0.0;
            double maxChildDesiredHeight = 0.0;
            Size childConstraint = new Size((availableSize.Width - SpaceSize.Width) / Columns, (availableSize.Height - SpaceSize.Height) / Rows);

            foreach (UIElement child in InternalChildren)
            {
                if (child.Visibility == Visibility.Collapsed)
                    continue;

                // Measure the child.
                child.Measure(childConstraint);
                Size childDesiredSize = child.DesiredSize;

                maxChildDesiredWidth = Math.Max(maxChildDesiredWidth, childDesiredSize.Width);
                maxChildDesiredHeight = Math.Max(maxChildDesiredHeight, childDesiredSize.Height);
            }

            return new Size(maxChildDesiredWidth * Columns + SpaceSize.Width, maxChildDesiredHeight * Rows + SpaceSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (RealChildCount == 0)
                return base.ArrangeOverride(finalSize);

            Rect childBounds = new Rect(0, 0, (finalSize.Width - SpaceSize.Width) / Columns, (finalSize.Height - SpaceSize.Height) / Rows);
            int RealIndex = 0;

            for (int index = 0; index < InternalChildren.Count; index++)
            {
                UIElement child = InternalChildren[index];

                if (child.Visibility != Visibility.Collapsed)
                {
                    int ColumnIndex = RealIndex % Columns;
                    int RowIndex = (int)Math.Floor((double)RealIndex / Columns);

                    childBounds.X = ColumnIndex * (childBounds.Width + Space);
                    childBounds.Y = RowIndex * (childBounds.Height + Space);

                    child.Arrange(childBounds);
                    RealIndex++;
                }
            }

            return new Size(childBounds.Width * Columns + SpaceSize.Width, childBounds.Height * Rows + SpaceSize.Height);
        }

        private Size GetSpaceSize(int _RealChildCount)
        {
            if (_RealChildCount == 0 || _RealChildCount == 1)
                return new Size();

            Size spaceSize = new Size();
            switch (MatrixType)
            {
                case MatrixType.SingleRow:
                    spaceSize.Width = (Columns - 1) * Space;
                    break;
                case MatrixType.SingleColumn:
                    spaceSize.Height = (Rows - 1) * Space;
                    break;
                case MatrixType.Auto:
                    spaceSize.Width = (Columns - 1) * Space;
                    spaceSize.Height = (Rows - 1) * Space;
                    break;
            }

            return spaceSize;
        }

        private void ComputedColumnAndRow(MatrixType _MatrixType, out int _RealChildCount, out int _Columns, out int _Rows)
        {
            _Columns = _Rows = _RealChildCount = 0;

            foreach (UIElement child in InternalChildren)
            {
                if (child.Visibility != Visibility.Collapsed)
                    _RealChildCount++;
            }

            if (_RealChildCount == 0)
                return;

            switch (_MatrixType)
            {
                case MatrixType.SingleRow:
                    _Rows = 1;
                    _Columns = _RealChildCount;
                    break;

                case MatrixType.SingleColumn:
                    _Rows = _RealChildCount;
                    _Columns = 1;
                    break;

                case MatrixType.Auto:
                    for (int index = 0; index < _RealChildCount; index++)
                    {
                        if (index == 0)
                            _Columns = _Rows = 1;
                        else
                        {
                            if (index == _Columns * _Rows)
                            {
                                if (_Columns == _Rows)
                                    _Columns++;
                                else
                                    _Rows++;
                            }
                        }
                    }
                    break;
            }
        }
    }
}
