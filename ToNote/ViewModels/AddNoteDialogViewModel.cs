namespace ToNote.ViewModels
{
    using System.Windows.Input;
    using ToNote.Interfaces;
    using ToNote.Logic;
    using ToNote.Models;

    public class AddNoteDialogViewModel : BaseDialogViewModel<Note>
    {
        public AddNoteDialogViewModel()
        {

        }

        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (_Name != value)
                {
                    _Name = value;

                    RaisePropertyChanged(nameof(Name));
                }
            }
        }

        private ICommand _AddNoteCommand;

        public ICommand AddNoteCommand
        {
            get
            {
                return _AddNoteCommand ?? (_AddNoteCommand = new RelayCommand<IDialogWindow>((window) => 
                {
                    if (!string.IsNullOrWhiteSpace(Name))
                    {
                        CloseDialogWithResult(window, new Note(Name));
                    }
                }));
            }
        }
    }
}
