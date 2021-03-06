﻿namespace ToNote.Controls
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
            this.PreviewKeyDown += (s, e) =>
            {
                if ((e.Key == Key.System ? e.SystemKey : e.Key) == Key.Back && Keyboard.Modifiers == (ModifierKeys.Alt | ModifierKeys.Shift))
                {
                    BackspacePressedWithAltShiftModifiers?.Invoke(this, new RoutedEventArgs());
                    e.Handled = true;
                }
            };

            this.TextChanged += (s, e) =>
            {
                //Prevents start of tracking while initializing.
                if (!this.IsLoaded) return;

                var str = new TextRange(this.Document.ContentStart, this.CaretPosition).Text;

                var lastChar = str.Length > 0 ? str[str.Length - 1] : ' ';

                if (!_tracking || lastChar == '/')
                {
                    var secondToLastChar = str.Length > 1 ? str[str.Length - 2] : ' ';

                    if (lastChar == '/' && (secondToLastChar == ' ' || secondToLastChar == '\n'))
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

                        var leftPos = this.CaretPosition;
                        for (int i = 0; i <= keyword.Length; ++i)
                        {
                            leftPos = leftPos.GetNextInsertionPosition(LogicalDirection.Backward) ?? leftPos;
                        }
                        // Delete the command from the textbox
                        var commandRange = new TextRange(leftPos, this.CaretPosition);
                        commandRange.Text = "";
                        CommandExecutionPointer = this.CaretPosition;

                        keywordAction.Action.Invoke();
                        break;
                    }
                }
            };

            this.PreviewMouseMove += (s, e) =>
            {
                var point = e.GetPosition(this);

                Mouse.OverrideCursor = point.X <= 5 ? Cursors.SizeAll : null;
            };

            this.MouseLeave += (s, e) =>
            {
                Mouse.OverrideCursor = null;
            };

            this.PreviewMouseLeftButtonDown += (s, e) =>
            {
                var point = e.GetPosition(this);

                if (point.X <= 5)
                    DragHandler.HandleMouseLeftButtonDown(s, e);
            };

            this.Drop += DragHandler.HandleDrop;

            this.PreviewDragOver += (s, e) =>
            {
                e.Handled = true;
            };
        }

        public event RoutedEventHandler BackspacePressedWithAltShiftModifiers;

        public TextPointer CommandExecutionPointer;

        public bool? Initializing { get; private set; } = false;

        private List<KeywordAction> _trackedKeywords { get; set; } = new List<KeywordAction>();

        private bool _tracking = false;
        private int _slashIndex = 0;
        private int _trackingCounter = 0;

        public string CurrentFile { get; set; }

        public TextRange TextRange => new TextRange(Document.ContentStart, Document.ContentEnd);

        public void ReadFromFile(string file)
        {
            if (!File.Exists(file)) return;

            Initializing = true;

            var text = new TextRange(this.Document.ContentStart, this.Document.ContentEnd);

            using (var stream = new FileStream(file, FileMode.Open))
            {
                text.Load(stream, DataFormats.Rtf);
            }

            CurrentFile = file;

            Initializing = false;
        }

        public void SetKeyboardFocus()
        {
            if (!this.IsLoaded)
            {
                this.Loaded += (s, e) =>
                {
                    this.Selection.Select(this.Selection.Start, this.Document.ContentEnd);
                    this.CaretPosition = this.Selection.End;
                    Keyboard.Focus(this);
                };
            }
            else
            {
                this.Selection.Select(this.Selection.Start, this.Document.ContentEnd);
                this.CaretPosition = this.Selection.End;
                Keyboard.Focus(this);
            }

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