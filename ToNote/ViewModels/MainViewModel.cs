namespace ToNote.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
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
            Notes = new ObservableCollection<Note>(IOHandler.DeserializeNotes());
            SelectedTags.CollectionChanged += (s, e) =>
            {
                FilteredNotes.Filter = x => Filter((Note)x);
            };
            SelectedNotes.CollectionChanged += (s, e) =>
            {
                FilteredNotes.Filter = x => Filter((Note)x);
            };

            
        }

        bool Filter(Note x) => SelectedTags.All(t => x.Tags.Contains(t)) && (SelectedNotes.Any() ? SelectedNotes.Any(n => n == x.Name) : true);

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

                    if (result != null && !Notes.Any(x => x.Name.Equals(result.Name)))
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

        private ICommand _OpenRemoveDialogCommand;

        public ICommand OpenRemoveDialogCommand
        {
            get
            {
                return _OpenRemoveDialogCommand ?? (_OpenRemoveDialogCommand = new RelayCommand(() =>
                {
                    var dialog = new RemoveNoteDialogViewModel();
                    dialog.Resizeable = false;

                    dialog.Title = "Remove";

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
                    var dialog = new RemoveNoteDialogViewModel();
                    dialog.Resizeable = false;
                    dialog.Title = "Confirm Delete";
                    DialogService.OpenDialog(dialog);

                    if (dialog.DialogResult == 0)
                    {
                        return;
                    }

                    //if (MessageBox.Show("Are you sure?", "Remove Note Confirmation", button: MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                    //    return;

                    IOHandler.RemoveNote(note);

                    Notes.Remove(note);
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

        private ICommand _OpenSettingsCommand;

        public ICommand OpenSettingsCommand
        {
            get
            {
                return _OpenSettingsCommand ?? (_OpenSettingsCommand = new RelayCommand(() =>
                {
                    var dialog = new SettingsViewModel();
                    dialog.ImportCommand = ImportCommand;
                    dialog.ExportCommand = ExportCommand;
                    dialog.Resizeable = false;

                    dialog.Title = "Settings";

                    DialogService.OpenDialog(dialog);
                }));
            }
        }

        private ICommand _DeleteTagCommand;

        public ICommand DeleteTagCommand
        {
            get => _DeleteTagCommand ?? (_DeleteTagCommand = new RelayCommand<object[]>(values =>
            {
                var note = values[0] as Note;
                var tag = values[1] as string;

                if (note.Tags.Contains(tag))
                {
                    note.Tags.Remove(tag);

                    if (!AllTags.Contains(tag))
                        SelectedTags.Remove(tag);

                    FilteredNotes.Refresh();
                }
            }));
        }

        private ICommand _ExportCommand;

        public ICommand ExportCommand
        {
            get => _ExportCommand ?? (_ExportCommand = new RelayCommand(() =>
            {
                IOHandler.ExportNotes(Notes);
            }));
        }

        private ICommand _ImportCommand;

        public ICommand ImportCommand
        {
            get => _ImportCommand ?? (_ImportCommand = new RelayCommand(() =>
            {
                var notes = IOHandler.ReadNotesFromFile();

                if (notes != null)
                    Notes = new ObservableCollection<Note>(notes);

                RaisePropertyChanged(nameof(FilteredNotes));
            }));
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

        public IEnumerable<string> AllTags => Notes.SelectMany(x => x.Tags).Distinct();

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
                    RaisePropertyChanged(nameof(AllTags));
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

        public IEnumerable<string> AllNotes => Notes.Select(x => x.Name);

        private bool _IsNotesPopupOpen;

        public bool IsNotesPopupOpen
        {
            get => _IsNotesPopupOpen;
            set
            {
                if (_IsNotesPopupOpen != value)
                {
                    _IsNotesPopupOpen = value;

                    RaisePropertyChanged(nameof(IsNotesPopupOpen));
                }
            }
        }

        private ICommand _ToggleNotesPopupCommand;

        public ICommand ToggleNotesPopupCommand
        {
            get
            {
                return _ToggleNotesPopupCommand ?? (_ToggleNotesPopupCommand = new RelayCommand(() =>
                {
                    IsNotesPopupOpen = !IsNotesPopupOpen;
                    RaisePropertyChanged(nameof(AllNotes));
                }));
            }
        }

        private ObservableCollection<string> _SelectedNotes;

        public ObservableCollection<string> SelectedNotes
        {
            get => _SelectedNotes ?? (_SelectedNotes = new ObservableCollection<string>());
            set
            {
                if (_SelectedNotes != value)
                {
                    _SelectedNotes = value;

                    RaisePropertyChanged(nameof(SelectedNotes));
                }
            }
        }

        private ICommand _ToggleNoteSelectionCommand;

        public ICommand ToggleNoteSelectionCommand
        {

            get
            {
                return _ToggleNoteSelectionCommand ?? (_ToggleNoteSelectionCommand = new RelayCommand<string>(note =>
                {
                    if (SelectedNotes.Contains(note))
                        SelectedNotes.Remove(note);
                    else
                        SelectedNotes.Add(note);
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
