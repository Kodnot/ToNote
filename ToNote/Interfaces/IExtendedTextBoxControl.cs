using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace ToNote.Interfaces
{
    public interface IExtendedTextBoxControl
    {
        event RoutedEventHandler BackspacePressedWithAltShiftModifiers;
        event KeyboardFocusChangedEventHandler GotKeyboardFocus;
        event TextChangedEventHandler TextChanged;
        event DragEventHandler Drop;
        event KeyboardFocusChangedEventHandler LostKeyboardFocus;

        TextRange TextRange { get; }
        string CurrentFile { get; }

        bool? Initializing { get; }

        void SetKeyboardFocus();
        void ReadFromFile(string file);

        void TrackKeyword(string keyword, Action action);
    }
}
