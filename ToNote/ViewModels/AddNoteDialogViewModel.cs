namespace ToNote.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using ToNote.Interfaces;
    using ToNote.Logic;
    using ToNote.Models;

    public class AddNoteDialogViewModel : BaseDialogViewModel<Note>
    {
        public AddNoteDialogViewModel()
        {

        }

        public Collection<Note> Notes { get; set; } = new Collection<Note>();

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

        private string _ErrorMessage;

        public string ErrorMessage
        {
            get => _ErrorMessage;
            set
            {
                if (_ErrorMessage != value)
                {
                    _ErrorMessage = value;

                    RaisePropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        private bool _ErrorVisible;

        public bool ErrorVisible
        {
            get => _ErrorVisible;
            set
            {
                if (_ErrorVisible != value)
                {
                    _ErrorVisible = value;

                    RaisePropertyChanged(nameof(ErrorVisible));
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
                        var forbiddenChars = "\\/:?<>|*\"".ToCharArray();

                        if (Notes.Any(x => x.Name.ToLower().Equals(Name.ToLower())))
                        {
                            ErrorVisible = true;
                            ErrorMessage = "Name already taken.";
                        }
                        else if (Name.IndexOfAny(forbiddenChars) != -1)
                        {
                            ErrorVisible = true;
                            ErrorMessage = "Note name cannot contain any of \\ / : &lt; &gt; &#34; &#42; ? | characters";
                        }
                        else
                            CloseDialogWithResult(window, new Note(Name));

                        
                    }
                }));
            }
        }
    }
}
