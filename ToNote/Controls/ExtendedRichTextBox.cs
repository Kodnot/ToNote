namespace ToNote.Controls
{
    using System;
    using System.Collections.Generic;
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

            this.TextChanged += (s, e) =>
            {
                //var range = new TextRange(Document.ContentStart, Document.ContentEnd);
                //Prevents start of tracking while initializing.
                if (!this.IsLoaded) return;

                var str = this.CaretPosition.GetTextInRun(LogicalDirection.Backward);

                if (!_tracking)
                {
                    var lastChar = str.Length > 0 ? str[str.Length - 1] : ' ';
                    var secondToLastChar = str.Length > 1 ? str[str.Length - 2] : ' ';

                    if (lastChar == '/' && secondToLastChar == ' ')
                    {
                        _tracking = true;
                        _slashIndex = str.Length - 1;
                    }
                }

                if (_tracking)
                {
                    //Stops tracking after 10 text changes without a '/' being the last character.
                    if (_trackingCounter++ > 10)
                    {
                        _tracking = false;
                        _trackingCounter = 0;
                    }

                   if (_slashIndex + 1 > str.Length)
                    {
                        _tracking = false;
                        _trackingCounter = 0;
                        return;
                    }

                    var keyword = str.Substring(_slashIndex + 1, str.Length - _slashIndex - 1);

                    foreach (var keywordAction in _trackedKeywords)
                    {
                        if (keyword != keywordAction.Keyword + ' ' && keyword != keywordAction.Keyword + '\r' + '\n') continue;

                        _tracking = false;
                        _trackingCounter = 0;

                        var range = new TextRange(this.CaretPosition.Paragraph.ContentStart, this.CaretPosition);

                        //Trims the '/' + keyword from the end of the text
                        range.Text = range.Text.Remove(_slashIndex > 0 ? _slashIndex - 1 : 0, _slashIndex > 0 ? range.Text.Length - _slashIndex + 1 : range.Text.Length);

                        CommandExecutionPointer = range.End;

                        keywordAction.Action.Invoke();
                    }
                }
            };
        }

        public event RoutedEventHandler BackspacePressedWhileEmpty;

        public TextPointer CommandExecutionPointer;

        private List<KeywordAction> _trackedKeywords { get; set; } = new List<KeywordAction>();

        private bool _tracking = false;
        private int _slashIndex = 0;
        private int _trackingCounter = 0;

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
            if (!this.IsLoaded)
            {
                this.Loaded += (s, e) =>
                {
                    Keyboard.Focus(this);
                };
            }
            else
                Keyboard.Focus(this);
        }
        
        public void TrackKeyword(string keyword, Action action)
        {
            var keywordAction = new KeywordAction() { Keyword = keyword, Action = action };

            _trackedKeywords.Add(keywordAction);
        }

        private struct KeywordAction
        {
            public string Keyword;
            public Action Action;
        }
    }
}