namespace ToNote.Controls
{
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using ToNote.Interfaces;

    public class ExtendedRichTextBox : RichTextBox, IExtendedTextBoxControl
    {
        public ExtendedRichTextBox()
        {
            // Checks if backspace was pressed when the textbox was empty and raises an event. Used for convenient empty textbox removal
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Back)
                {
                    var range = new TextRange(Document.ContentStart, Document.ContentEnd);
                    if (string.IsNullOrWhiteSpace(range.Text))
                        BackspacePressedWhileEmpty?.Invoke(this, new RoutedEventArgs());
                }
            };

        }

        public event RoutedEventHandler BackspacePressedWhileEmpty;

        public string CurrentFile { get; private set; }

        public TextRange TextRange => new TextRange(Document.ContentStart, Document.ContentEnd);

        public void ReadFromFile(string file)
        {
            if (!File.Exists(file)) return;

            var text = new TextRange(this.Document.ContentStart, this.Document.ContentEnd);

            using (var stream = new FileStream(file, FileMode.Open))
            {
                text.Load(stream, DataFormats.Rtf);
            }

            CurrentFile = file;
        }

        public void SetKeyboardFocus()
        {
            Keyboard.Focus(this);
        }
    }
}