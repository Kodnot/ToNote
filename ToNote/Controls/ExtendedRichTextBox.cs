namespace ToNote.Controls
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    public class ExtendedRichTextBox : RichTextBox
    {
        public ExtendedRichTextBox()
        {
            var range = new TextRange(Document.ContentStart, Document.ContentEnd);

            // Checks if backspace was pressed when the textbox was empty and raises an event. Used for convenient empty textbox removal
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Back)
                {
                    if (string.IsNullOrWhiteSpace(range.Text))
                        BackspacePressedWhileEmpty?.Invoke(this, new RoutedEventArgs());
                }
            };

            this.TextChanged += (s, e) =>
            {
                //Prevents start of tracking while initializing.
                if (this.IsLoaded)
                {
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

                        foreach (var keywordaction in _trackedKeywords)
                        {
                            if (keyword != keywordaction.Keyword + ' ') return;

                            _tracking = false;

                            //Trims the '/' + keyword from the end of the text
                            range.Text = range.Text.Remove(offset - keyword.Length, keyword.Length + 1);

                            keywordaction.Action.Invoke();

                            return;
                        }
                    }

                    if (lastChar == '/' && secondToLastChar == ' ')
                    {
                        _tracking = true;
                        _slashIndex = offset;
                        _trackingCounter = 0;
                    }
                }
            };
        }

        public RoutedEventHandler BackspacePressedWhileEmpty;

        private List<KeywordAction> _trackedKeywords { get; set; } = new List<KeywordAction>();

        private bool _tracking = false;
        private int _slashIndex = 0;
        private int _trackingCounter = 0;

        public string CurrentFile { get; private set; }

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

        public void TrackKeyword(string keyword, Action action)
        {
            var keywordaction = new KeywordAction() { Keyword = keyword, Action = action };
            if (!_trackedKeywords.Contains(keywordaction))
                _trackedKeywords.Add(keywordaction);
        }

        private struct KeywordAction
        {
            public string Keyword;
            public Action Action;
        }
    }
}