namespace ToNote.ViewModels
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
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

                SelectedTags.CollectionChanged += (s, e) =>
                {
                    FilteredNotes.Filter = x => SelectedTags.All(t => ((Note)x).Tags.Contains(t));
                };
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
                    while (note.FileNames.Any())
                        note.DeleteFile(note.FileNames.First());

                    var metadataFileName = note.Name + "Metadata.txt";
                    if (File.Exists(metadataFileName))
                        File.Delete(metadataFileName);

                    var directoryPath = Path.Combine("Data", note.Name);

                    if (Directory.Exists(directoryPath))
                        Directory.Delete(directoryPath, true);
                }));
            }
        }

        private ICommand _AddTagCommand;

        public ICommand AddTagCommand
        {
            get
            {
                return _AddTagCommand ?? (_AddTagCommand = new RelayCommand<Note>(note =>
                {
                    if (!string.IsNullOrWhiteSpace(note.TagName) && !note.Tags.Contains(note.TagName))
                    {
                        note.Tags.Add(note.TagName);
                        note.TagName = "";
                    }

                    RaisePropertyChanged(nameof(AllTags));
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

        private ObservableCollection<string> _SelectedTags;

        public ObservableCollection<string> SelectedTags
        {
            get => _SelectedTags ?? (_SelectedTags = new ObservableCollection<string>());
            set
            {
                if (_SelectedTags != value)
                {
                    _SelectedTags = value;

                    RaisePropertyChanged(nameof(SelectedTags));
                }
            }
        }

        public IEnumerable AllTags => Notes.SelectMany(x => x.Tags).Distinct();

        private bool _IsGroupingPanelOpen;

        public bool IsGroupingPanelOpen
        {
            get => _IsGroupingPanelOpen;
            set
            {
                if (_IsGroupingPanelOpen != value)
                {
                    _IsGroupingPanelOpen = value;

                    RaisePropertyChanged(nameof(IsGroupingPanelOpen));
                }
            }
        }

        private ICommand _ToggleGroupingPanelCommand;

        public ICommand ToggleGroupingPanelCommand
        {
            get
            {
                return _ToggleGroupingPanelCommand ?? (_ToggleGroupingPanelCommand = new RelayCommand(() =>
                {
                    IsGroupingPanelOpen = !IsGroupingPanelOpen;
                }));
            }
        }

        private ICommand _ToggleTagSelectionCommand;

        public ICommand ToggleTagSelectionCommand
        {

            get
            {
                return _ToggleTagSelectionCommand ?? (_ToggleTagSelectionCommand = new RelayCommand<string>(tag =>
                {
                    if (SelectedTags.Contains(tag))
                        SelectedTags.Remove(tag);
                    else
                        SelectedTags.Add(tag);
                }));
            }
        }

        public ICollectionView FilteredNotes => CollectionViewSource.GetDefaultView(Notes);

        private bool _IsSelected;

        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    RaisePropertyChanged(nameof(IsSelected));
                    FilteredNotes.Refresh();
                }
            }
        }
    }
}
