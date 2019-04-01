namespace ToNote.Controls
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using ToNote.Models;
    using ToNote.Views;

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
                if (sender is NoteView noteView)
                {
                    var panel = (FindAncestorOfType<ItemsControl>(sender as DependencyObject) as ItemsControl).ItemsSource as ObservableCollection<Note>;

                    var newIndex = panel.IndexOf(noteView.DataContext as Note);

                    var note = (source as NoteView).DataContext as Note;

                    panel.RemoveAt(panel.IndexOf(note));

                    panel.Insert(newIndex, note);

                    args.Handled = true;
                }

                else
                {
                    var panel = FindAncestorOfType<ItemsControl>(sender as DependencyObject) as ItemsControl;

                    var todoControl = FindAncestorOfType<TodoControl>((DependencyObject)sender);

                    int newIndex;

                    if (todoControl != null)
                        newIndex = panel.Items.IndexOf(todoControl);
                    else
                        newIndex = panel.Items.IndexOf(sender);

                    if (panel.Items.IndexOf(source) != -1)
                    {
                        panel.Items.RemoveAt(panel.Items.IndexOf(source));
                        panel.Items.Insert(newIndex, source);
                        args.Handled = true;
                    }
                }
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
