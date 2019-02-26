namespace ToNote.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    public class ExtendedRichTextBox : RichTextBox
    {
        public ExtendedRichTextBox()
        {
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Back)
                {
                    var range = new TextRange(Document.ContentStart, Document.ContentEnd);
                    if (string.IsNullOrWhiteSpace(range.Text))
                        BackspacePressedWhileEmpty?.Invoke(this, new RoutedEventArgs());
                }
            };
        }

        public RoutedEventHandler BackspacePressedWhileEmpty;
    }
}