﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace TestUI.Utils
{
    [MarkupExtensionReturnType(typeof(Visibility))]
    public class BubbleVisibilityExtension : MarkupExtension
    {
        public BubbleVisibilityExtension()
        {

        }

        public BubbleVisibilityExtension(BindingBase binding, Type ancestorType)
        {
            _Binding = binding;
            _AncestorType = ancestorType;
        }

        private BindingBase _Binding;
        [ConstructorArgument("binding")]
        public BindingBase Binding
        {
            get { return _Binding; }
            set { _Binding = value; }
        }             

        private Type _AncestorType = null;
        [ConstructorArgument("ancestorType")]
        public Type AncestorType
        {
            get { return _AncestorType; }
            set { _AncestorType = value; }
        }

        private uint _AncestorLevel = 0;
        public uint AncestorLevel
        {
            get { return _AncestorLevel; }
            set { _AncestorLevel = value; }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (service == null)
                return null;

            UIElement mTarget = service.TargetObject as FrameworkElement;
            DependencyProperty mProperty = service.TargetProperty as DependencyProperty;
            if (mTarget != null)
            {
                var v = DependencyPropertyDescriptor.FromProperty(mProperty, typeof(UIElement));
                v.AddValueChanged(mTarget, VisibilityChanged);

                if (mProperty != null && _Binding != null)
                {
                    return _Binding.ProvideValue(serviceProvider);// BindingOperations.SetBinding(mTarget, mProperty, _Binding);
                }
            }

            return Visibility.Visible;
        }

        private void VisibilityChanged(object sender, EventArgs e)
        {
            if (_AncestorType == null || _Binding == null || _AncestorLevel == 0)
                return;

            UIElement element = sender as UIElement;
            ssss(element, element.Visibility);
        }

        private void ssss(UIElement element, Visibility visibility)
        {
            DependencyObject parentNode = VisualTreeHelper.GetParent(element);
            uint FoundAncestorLevel = 0;

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

                if (_AncestorType.IsAssignableFrom(parentNode.GetType()))
                {
                    FoundAncestorLevel++;
                    if (FoundAncestorLevel == _AncestorLevel)
                        break;
                }
                parentNode = VisualTreeHelper.GetParent(parentNode);
            }
        }

        private Visibility GetAndSetParentVisibility<T>(T obj) where T : FrameworkElement, IAddChild
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
}
