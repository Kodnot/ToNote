using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ToNote.Controls
{
    public static class DragHandler
    {
        public static void HandleMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            if (args.LeftButton == MouseButtonState.Pressed)
            {
                var data = new DataObject();
                var todoControl = FindAncestorOfType<TodoControl>(sender as DependencyObject);
                if (todoControl != null)
                {
                    data.SetData("Source", todoControl);
                }
                else
                    data.SetData("Source", sender as DependencyObject);

                DragDrop.DoDragDrop(sender as DependencyObject, data, DragDropEffects.Move);
                args.Handled = true;
            }
        }

        public static void HandleDrop(object sender, DragEventArgs args)
        {
            var source = args.Data.GetData("Source") as DependencyObject;
            if (source != null)
            {
                var panel = FindAncestorOfType<ItemsControl>(sender as DependencyObject) as ItemsControl;

                var newIndex = 0;

                foreach (var item in panel.Items)
                {
                    if (item == sender)
                    {
                        newIndex = panel.Items.IndexOf(item);
                    }
                }

                panel.Items.RemoveAt(panel.Items.IndexOf(source));
                panel.Items.Insert(newIndex, source);
                args.Handled = true;
            }
        }

        public static DependencyObject FindAncestorOfType<T>(DependencyObject obj)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent;
        }
    }
}
