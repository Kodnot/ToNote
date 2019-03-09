namespace ToNote.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using ToNote.Models;

    public class TodoControl : ContentControl
    {
        public TodoControl(Todo todo)
        {
            this.Content = todo;

            this.Loaded += (s, e) =>
            {
                //Gets the DataTemplate defined for the Todo objects.
                var template = this.ContentTemplate;

                if (template == null) return;

                //Looks for an ExtendedRichTextBox with an x:Name="rtb" attribute from the logical children of the generated ContentPresenter which has the DataTemplate applied to it.
                var rtb = (ExtendedRichTextBox)template.FindName("rtb", (FrameworkElement)VisualTreeHelper.GetChild(this, 0));

                if (rtb == null) return;

                //If an ExtendedRichTextBox is found, hooks to the BackspacePressedWhileEmpty event.
                rtb.BackspacePressedWhileEmpty += (o, a) =>
                {
                    this.BackspacePressedWhileEmpty?.Invoke(this, new RoutedEventArgs());
                };
            };
        }

        public RoutedEventHandler BackspacePressedWhileEmpty;
    }
}
