using System.Windows.Input;

namespace ToNote.ViewModels
{
    class SettingsViewModel : BaseDialogViewModel<int>
    {
        public ICommand ImportCommand { get; set;}
        public ICommand ExportCommand { get; set;}

    }
}
