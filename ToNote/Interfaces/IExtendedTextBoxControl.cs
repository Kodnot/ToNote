using System.Windows;
using System.Windows.Input;

namespace ToNote.Interfaces
{
    public interface IExtendedTextBoxControl
    {
        event RoutedEventHandler BackspacePressedWhileEmpty;
        event KeyboardFocusChangedEventHandler GotKeyboardFocus;

        string CurrentFile { get; }

        void SetKeyboardFocus();
    }
}
