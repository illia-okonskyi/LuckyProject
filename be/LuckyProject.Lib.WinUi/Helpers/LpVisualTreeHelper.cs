using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;

namespace LuckyProject.Lib.WinUi.Helpers
{
    public static class LpVisualTreeHelper
    {
        public static T FindControlByName<T>(UIElement parent, string name)
            where T : FrameworkElement
        {
            if (parent == null)
            {
                return null;
            } 

            if (parent.GetType() == typeof(T) && ((T)parent).Name == name)
            {
                return (T)parent;
            }

            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = (UIElement)VisualTreeHelper.GetChild(parent, i);
                var result = FindControlByName<T>(child, name);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static T FindVisibleControlByTag<T>(UIElement parent, string tag)
            where T : FrameworkElement
        {
            if (parent == null)
            {
                return null;
            }

            if (parent.GetType() == typeof(T))
            {
                var target = (T)parent;
                if (target.Visibility == Visibility.Visible && (string)target.Tag == tag)
                {
                    return target;
                }
            }

            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = (UIElement)VisualTreeHelper.GetChild(parent, i);
                var result = FindVisibleControlByTag<T>(child, tag);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static List<T> GetDirectChildsOfType<T>(UIElement parent)
            where T : FrameworkElement
        {
            var result = new List<T>();
            if (parent == null)
            {
                return result;
            }

            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = (UIElement)VisualTreeHelper.GetChild(parent, i);
                if (child.GetType() == typeof(T))
                {
                    result.Add((T)child);
                }
            }

            return result;
        }

        public static T FindParentOfType<T>(DependencyObject o)
            where T : DependencyObject
        {
            if (o == null)
            {
                return null;
            }

            var parent = VisualTreeHelper.GetParent(o);
            while (parent != null)
            {
                if (parent.GetType() == typeof(T))
                {
                    return (T)parent;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
    }
}
