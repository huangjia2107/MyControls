using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyControl.Helper;
using MyControl.Resources.Controls;

namespace MyControl
{
    public class ShowText
    {
        public int str { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UserWindow
    {
        AdornerLayer mAdornerLayer = null;
        ObservableCollection<ShowText> intCollection = new ObservableCollection<ShowText>();

        internal static class Utils
        {
            public static T FindVisualParent<T>(DependencyObject obj) where T : class
            {
                while (obj != null)
                {
                    if (obj is T)
                        return obj as T;

                    obj = VisualTreeHelper.GetParent(obj);
                }

                return null;
            }
        }

        ListBoxItem mSelectedItem = null;
        ListBoxItem mHoveredItem = null;
        DateTime mStartHoverTime = DateTime.MinValue;

        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 12; i++)
            {
                intCollection.Add(new ShowText { str = (i + 1) });
            } 
            mListBox.ItemsSource = intCollection;
        }

        private void MainWindow_OnOpenSetting(object sender, EventArgs e)
        {
            MyControl.Resources.Controls.MessageBox.Show("Open Setting.");
        }

        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
                return;

            Point pos = e.GetPosition(mListBox);
            HitTestResult result = VisualTreeHelper.HitTest(mListBox, pos);
            if (result == null)
                return;

            ListBoxItem listBoxItem = Utils.FindVisualParent<ListBoxItem>(result.VisualHit); // Find your actual visual you want to drag
            if (listBoxItem == null || listBoxItem.Content != mListBox.SelectedItem || !(mListBox.SelectedItem is ShowText))
                return;

            Debug.WriteLine("Add AdornerLayer");
            mSelectedItem = listBoxItem;
            Point posRelative = e.GetPosition(listBoxItem);
            DragDropAdorner adorner = new DragDropAdorner(listBoxItem, posRelative);
            mAdornerLayer = AdornerLayer.GetAdornerLayer(mListBox); // Window class do not have AdornerLayer
            mAdornerLayer.Add(adorner);

            // Here, we should notice that dragsource param will specify on which 
            // control the drag&drop event will be fired 
            System.Windows.DragDrop.DoDragDrop(mListBox, listBoxItem.Content, DragDropEffects.Move);

            Debug.WriteLine("Remove AdornerLayer");
            mAdornerLayer.Remove(adorner);
            mAdornerLayer = null;
            mSelectedItem = mHoveredItem = null;
            mStartHoverTime = DateTime.MinValue;
        }

        private void ListBox_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (mAdornerLayer != null)
                mAdornerLayer.Update();

            UpdateViewState();
        }

        private void UpdateViewState()
        {
            if (mSelectedItem == null)
                return;

            Win32.POINT point = new Win32.POINT();
            if (Win32.GetCursorPos(ref point))
            {
                Point pos = new Point(point.X, point.Y);
                pos = mListBox.PointFromScreen(pos);
                HitTestResult result = VisualTreeHelper.HitTest(mListBox, pos);
                if (result != null)
                {
                    ListBoxItem CurrentItem = Utils.FindVisualParent<ListBoxItem>(result.VisualHit);
                    if (CurrentItem != null && CurrentItem != mSelectedItem)
                    {
                        if (mHoveredItem != CurrentItem)
                        {
                            Debug.WriteLine("mHoveredItem != CurrentItem");
                            mHoveredItem = CurrentItem;
                            mStartHoverTime = DateTime.Now;
                        }
                        else
                        {
                            Debug.WriteLine("mHoveredItem == CurrentItem");
                            if (DateTime.Now - mStartHoverTime > TimeSpan.FromMilliseconds(300))
                            {
                                Debug.WriteLine("Move");
                                int HoveredIndex = mListBox.Items.IndexOf((ShowText)(mHoveredItem.Content));
                                int SelectedIndex = mListBox.Items.IndexOf((ShowText)(mSelectedItem.Content));

                                //Debug.WriteLine("SelectedIndex:" + mListBox.SelectedIndex + "    HoveredIndex:" + HoveredIndex);
                                intCollection.Move(SelectedIndex, HoveredIndex);

                                mHoveredItem = null;
                                mStartHoverTime = DateTime.MinValue;
                            }
                        }
                    }
                }
            }
        }

        #region 滚动内容拖动

        Point? lastDragPoint = null;

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            sv.Cursor = Cursors.Arrow;
            lastDragPoint = null;
        }

        private void UIElement_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            var mousePos = e.GetPosition(sv);
            if (mousePos.X <= sv.ViewportWidth && mousePos.Y < sv.ViewportHeight && sv.ComputedVerticalScrollBarVisibility == Visibility.Visible)
            {
                sv.Cursor = Cursors.Hand;
                lastDragPoint = mousePos;
            }
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            sv.Cursor = Cursors.Arrow;
            lastDragPoint = null;
        }

        private void UIElement_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            if (lastDragPoint.HasValue)
            {
                Point posNow = e.GetPosition(sv);
                double dX = posNow.X - lastDragPoint.Value.X;
                double dY = posNow.Y - lastDragPoint.Value.Y;

                lastDragPoint = posNow;
                sv.ScrollToHorizontalOffset(sv.HorizontalOffset - dX);
                sv.ScrollToVerticalOffset(sv.VerticalOffset - dY);
            }
        }

        #endregion

        /*

        #region 窗口的拖动、最小化、最大化、还原、关闭
         
        Rect rcnormal;//定义一个全局rect记录还原状态下窗口的位置和大小。
         
        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (this.ActualWidth == SystemParameters.WorkArea.Width)
                    MaxBtn.IsChecked = false;
                else
                    MaxBtn.IsChecked = true;
                MaxBtn_OnClick(null, null);
                return;
            }

            if (MaxBtn.IsChecked == true)
                return;

            this.DragMove();
        }

        private void MiniBtn_OnClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseBtn_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MaxBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (MaxBtn.IsChecked == true)
            {
                rcnormal = new Rect(this.Left, this.Top, this.Width, this.Height);//保存下当前位置与大小
                this.Left = 0;//设置位置
                this.Top = 0;
                Rect rc = SystemParameters.WorkArea;//获取工作区大小
                this.Width = rc.Width;
                this.Height = rc.Height;
            }
            else
            {
                this.Left = rcnormal.Left;
                this.Top = rcnormal.Top;
                this.Width = rcnormal.Width;
                this.Height = rcnormal.Height;
            }
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualHeight > SystemParameters.WorkArea.Height || this.ActualWidth > SystemParameters.WorkArea.Width)
            {
                this.WindowState = System.Windows.WindowState.Normal;
                MaxBtn.IsChecked = true;
                MaxBtn_OnClick(null, null);
            }
        }

        #endregion    
        */

       
    }
}
