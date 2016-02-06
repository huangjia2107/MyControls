using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MyControl.Helper
{
    public class PanelLayoutAnimator
    {

        public PanelLayoutAnimator(Panel panelToAnimate)
        {
            _panel = panelToAnimate;
            _panel.LayoutUpdated += PanelLayoutUpdated;
        }

        public static readonly DependencyProperty IsAnimationEnabledProperty
           = DependencyProperty.RegisterAttached("IsAnimationEnabled", typeof(bool), typeof(PanelLayoutAnimator),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsAnimationEnabledInvalidated)));
        public static void SetIsAnimationEnabled(DependencyObject dependencyObject, bool enabled)
        {
            dependencyObject.SetValue(IsAnimationEnabledProperty, enabled);
        }


        /// <summary>
        /// Detaches this animator from the panel
        /// </summary>
        public void Detach()
        {
            if (_panel != null)
            {
                _panel.LayoutUpdated -= PanelLayoutUpdated;
                _panel = null;
            }
        }

        /// <summary>
        /// Called when panel's layout is updated
        /// </summary>
        /// <remarks>
        /// Note: This is actually called when any layouts are updated
        /// </remarks>
        private void PanelLayoutUpdated(object sender, EventArgs e)
        {
            // At this point, the panel has moved the children to the new locations, but hasn't
            // been rendered
            foreach (UIElement child in _panel.Children)
            {
                // Figure out where child actually is right now. This is a combination of where the
                // panel put it and any render transform currently applied
                Point currentPosition = child.TransformToAncestor(_panel).Transform(new Point());

                // See what transform is being applied
                Transform currentTransform = child.RenderTransform;

                // Compute where the panel actually arranged it to
                Point arrangePosition = currentPosition;
                if (currentTransform != null)
                {
                    // Undo any transform we applied
                    arrangePosition = currentTransform.Inverse.Transform(arrangePosition);
                }

                // If we had previously stored an arrange position, see if it has moved
                if (child.ReadLocalValue(SavedArrangePositionProperty) != DependencyProperty.UnsetValue)
                {
                    Point savedArrangePosition = (Point)child.GetValue(SavedArrangePositionProperty);

                    // If the arrange position hasn't changed, then we've already set up animations, etc
                    // and don't need to do anything
                    if (!AreReallyClose(savedArrangePosition, arrangePosition))
                    {
                        // If we apply the current transform to the saved arrange position, we'll see where
                        // it was last rendered
                        Point lastRenderPosition = currentTransform.Transform(savedArrangePosition);

                        // Transform the child from the new location back to the old position
                        TranslateTransform newTransform = new TranslateTransform();
                        child.RenderTransform = newTransform;

                        // Decay the transformation with an animation
                        newTransform.BeginAnimation(TranslateTransform.XProperty, MakeAnimation(lastRenderPosition.X - arrangePosition.X));
                        newTransform.BeginAnimation(TranslateTransform.YProperty, MakeAnimation(lastRenderPosition.Y - arrangePosition.Y));
                    }
                }

                // Save off the previous arrange position
                child.SetValue(SavedArrangePositionProperty, arrangePosition);
            }
        }

        /// <summary>
        /// Called when IsAnimationEnabled is changed on an object.
        /// </summary>
        private static void OnIsAnimationEnabledInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Panel panel = dependencyObject as Panel;
            if (panel != null)
            {
                if ((bool)e.NewValue)
                {
                    // Turn animations on for the panel if there's not already a PanelAnimator attached
                    if (panel.ReadLocalValue(AttachedAnimatorProperty) == DependencyProperty.UnsetValue)
                    {
                        PanelLayoutAnimator animator = new PanelLayoutAnimator(panel);
                        panel.SetValue(AttachedAnimatorProperty, animator);
                    }
                }
                else
                {
                    // clear animations
                    if (panel.ReadLocalValue(AttachedAnimatorProperty) != DependencyProperty.UnsetValue)
                    {
                        PanelLayoutAnimator animator = (PanelLayoutAnimator)panel.ReadLocalValue(AttachedAnimatorProperty);
                        animator.Detach();
                        panel.SetValue(AttachedAnimatorProperty, DependencyProperty.UnsetValue);
                    }
                }
            }
        }

        // Check if two points are really close. If you don't do epsilon comparisons, you can get lost in the
        // noise of floating point operations
        private bool AreReallyClose(Point p1, Point p2)
        {
            return (Math.Abs(p1.X - p2.X) < .001 && Math.Abs(p1.Y - p2.Y) < .001);
        }

        // Create an animation to decay from start to 0 over .5 seconds
        private static DoubleAnimation MakeAnimation(double start)
        {
            DoubleAnimation animation = new DoubleAnimation(start, 0d, new Duration(TimeSpan.FromMilliseconds(300)));
            animation.AccelerationRatio = 0.2;
            return animation;
        }

        // dependency property we attach to children to save their last arrange position
        private static readonly DependencyProperty SavedArrangePositionProperty
           = DependencyProperty.RegisterAttached("SavedArrangePosition", typeof(Point), typeof(PanelLayoutAnimator));

        // dependency property we attach to panels to save a reference to ourselves
        private static readonly DependencyProperty AttachedAnimatorProperty
           = DependencyProperty.RegisterAttached("AttachedAnimator", typeof(PanelLayoutAnimator), typeof(PanelLayoutAnimator));

        private Panel _panel;
    }
}
