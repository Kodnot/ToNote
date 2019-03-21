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
    using System.Windows;

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
                    dialog.Resizeable = false;
                    dialog.Title = "Add a note";

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

        private ICommand _RemoveNoteCommand;

        public ICommand RemoveNoteCommand
        {
            get
            {
                return _RemoveNoteCommand ?? (_RemoveNoteCommand = new RelayCommand<Note>(note =>
                {
                    if (!Notes.Contains(note))
                        return;

                    if (MessageBox.Show("Are you sure?", "Remove Note Confirmation", button: MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                        return;

                    Notes.Remove(note);

                    // TODO: This should be handled by the IO handler class instead.
                    while(note.FileNames.Any())
                        note.DeleteFile(note.FileNames.First());

                    var metadataFileName = note.Name + "Metadata.txt";
                    if (File.Exists(metadataFileName))
                        File.Delete(metadataFileName);

                    var todosFileName = "*_" + note.Name + "_TODO";

                    var todoFiles = Directory.GetFiles("Data", todosFileName);

                    foreach (var file in todoFiles)
                    {
                        if (File.Exists(file)) File.Delete(file);
                    }
                }));
            }
        }

        private ICommand _OpenAboutPageCommand;

        public ICommand OpenAboutPageCommand
        {
            get
            {
                return _OpenAboutPageCommand ?? (_OpenAboutPageCommand = new RelayCommand(() =>
                {
                    var dialog = new AboutPageViewModel();
                    dialog.Resizeable = false;

                    dialog.Title = "About";

                    DialogService.OpenDialog(dialog);
                }));
            }
        }
    }
}
