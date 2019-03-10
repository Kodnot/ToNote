using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace ToNote.Interfaces
{
    public interface IExtendedTextBoxControl
    {
        event RoutedEventHandler BackspacePressedWhileEmpty;
        event KeyboardFocusChangedEventHandler GotKeyboardFocus;

        TextRange TextRange { get; }
        string CurrentFile { get; }

        void SetKeyboardFocus();
        void ReadFromFile(string file);
    }
}
