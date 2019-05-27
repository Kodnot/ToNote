using System.Windows.Input;
using ToNote.Interfaces;
using ToNote.Logic;

namespace ToNote.ViewModels
{
    class RemoveNoteDialogViewModel : BaseDialogViewModel<int>
    {
        private ICommand _RemoveCommand;

        public ICommand RemoveCommand
        {
            get
            {
                return _RemoveCommand ?? (_RemoveCommand = new RelayCommand<IDialogWindow>((window) =>
                {
                    CloseDialogWithResult(window, 1);
                    
                }));
            }
        }

        private ICommand _CancelCommand;

        public ICommand CancelCommand
        {
            get
            {
                return _CancelCommand ?? (_CancelCommand = new RelayCommand<IDialogWindow>((window) =>
                {
                    CloseDialogWithResult(window, 0);
                }));
            }
        }
    }

    
}
