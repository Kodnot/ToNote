using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ToNoteTests.Core
{
    public class VisualTreeService
    {
        //Source: https://stopbyte.com/t/how-to-find-wpf-child-control-by-name-or-type/136/3
        //Modified to use a predicate

        public static T FindChild<T>(DependencyObject parent, Predicate<T> predicate = null)
   where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;
            T foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, predicate);
                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (predicate == null)
                {
                    {
                        // child element found.
                        foundChild = (T)child;
                        break;
                    }
                }
                else if (predicate.Invoke((T)child))
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
                
            }
            return foundChild;
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
