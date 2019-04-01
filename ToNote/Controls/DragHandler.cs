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

                data.SetData("Source", todoControl ?? sender as DependencyObject);

                DragDrop.DoDragDrop(sender as DependencyObject, data, DragDropEffects.Move);
                args.Handled = true;
            }
        }

        public static void HandleDrop(object sender, DragEventArgs args)
        {
            var source = args.Data.GetData("Source") as DependencyObject;

            if (source == null) return;

            if (sender is NoteView noteView && source is NoteView sourceView)
            {
                var panel = (FindAncestorOfType<ItemsControl>(sender as DependencyObject) as ItemsControl).ItemsSource as ObservableCollection<Note>;

                var newIndex = panel.IndexOf(noteView.DataContext as Note);

                var note = sourceView.DataContext as Note;

                panel.RemoveAt(panel.IndexOf(note));

                panel.Insert(newIndex, note);
            }

            else
            {
                var panel = FindAncestorOfType<ItemsControl>(sender as DependencyObject) as ItemsControl;

                var todoControl = FindAncestorOfType<TodoControl>((DependencyObject)sender);

                int newIndex = todoControl != null ? panel.Items.IndexOf(todoControl) : panel.Items.IndexOf(sender);

                if (panel.Items.IndexOf(source) != -1)
                {
                    panel.Items.RemoveAt(panel.Items.IndexOf(source));
                    panel.Items.Insert(newIndex, source);
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
