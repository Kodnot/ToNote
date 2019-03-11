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
                var range = new TextRange(Document.ContentStart, Document.ContentEnd);
                //Prevents start of tracking while initializing.
                if (!this.IsLoaded) return;

                // -3, because text ending has \r\n chars
                var offset = range.Text.Length - 3;
                var secondToLastChar = offset - 1 >= 0 ? range.Text[offset - 1] : ' ';
                var lastChar = offset >= 0 ? range.Text[offset] : ' ';

                if (_tracking)
                {
                    //Stops tracking after 10 text changes without a '/' being the last character.
                    if (_trackingCounter++ > 10)
                    {
                        _tracking = false;
                    }

                    //If text is deleted beyond '/', stops tracking
                    if (offset - _slashIndex < 0)
                    {
                        _tracking = false;
                        return;
                    }

                    var keyword = range.Text.Substring(_slashIndex + 1, offset - _slashIndex);

                    foreach (var keywordAction in _trackedKeywords)
                    {
                        if (keyword != keywordAction.Keyword + ' ' && keyword != keywordAction.Keyword + '\r' + '\n') continue;

                        _tracking = false;

                        //Trims the '/' + keyword from the end of the text
                        range.Text = range.Text.Remove(offset - keyword.Length, keyword.Length + 1);

                        keywordAction.Action.Invoke();
                    }
                }

                if (lastChar == '/' && secondToLastChar == ' ' || secondToLastChar == '\n')
                {
                    _tracking = true;
                    _slashIndex = offset;
                    _trackingCounter = 0;
                }
            };
        }

        public event RoutedEventHandler BackspacePressedWhileEmpty;

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