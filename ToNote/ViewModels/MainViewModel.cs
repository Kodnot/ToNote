namespace ToNote.ViewModels
{
    using Newtonsoft.Json;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;
    using ToNote.Logic;
    using ToNote.Logic.Dialog;
    using ToNote.Models;

    public class MainViewModel : BaseModel
    {
        public MainViewModel()
        {
            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory()).Where(x => Path.GetFileName(x).Contains("Metadata.txt")))
            {
                using (var reader = new StreamReader(file))
                    Notes.Add(JsonConvert.DeserializeObject<Note>(reader.ReadToEnd()));
            }
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
