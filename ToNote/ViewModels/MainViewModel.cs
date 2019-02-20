namespace ToNote.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using ToNote.Logic;
    using ToNote.Logic.Dialog;
    using ToNote.Models;

    public class MainViewModel : BaseModel
    {
        public MainViewModel()
        {

        }

        private ObservableCollection<Note> _Notes;

        public ObservableCollection<Note> Notes
        {
            get
            {
                return _Notes ?? (_Notes = new ObservableCollection<Note>());
            }
            set
            {
                if (_Notes != value)
                {
                    _Notes = value;

                    RaisePropertyChanged(nameof(Notes));
                }
            }
        }

        private ICommand _AddNoteCommand;

        public ICommand AddNoteCommand
        {
            get
            {
                return _AddNoteCommand ?? (_AddNoteCommand = new RelayCommand(() => 
                {
                    var dialog = new AddNoteDialogViewModel();

                    var result = DialogService.OpenDialog(dialog);

                    if (result != null)
                        Notes.Add(result);
                }));
            }
        }

        private ICommand _EditNoteCommand;

        public ICommand EditNoteCommand
        {
            get
            {
                return _EditNoteCommand ?? (_EditNoteCommand = new RelayCommand<Note>((note) =>
                {
                    var dialog = new EditNoteDialogViewModel();
                    dialog.Note = note;

                    DialogService.OpenDialog(dialog);
                }));
            }
        }
    }
}
