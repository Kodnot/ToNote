using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ToNote.Interfaces;
using ToNote.Logic;

namespace ToNote.ViewModels
{
    class RemoveNoteDialogViewModel : BaseDialogViewModel<int>
    {
        //public ICommand RemoveCommand { get; set; }
       // public ICommand CancelCommand { get; set; }

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
