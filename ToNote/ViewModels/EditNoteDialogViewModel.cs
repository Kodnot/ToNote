namespace ToNote.ViewModels
{
    using ToNote.Models;

    public class EditNoteDialogViewModel : BaseDialogViewModel<bool>
    {
        public EditNoteDialogViewModel()
        {

        }

        private Note _Note;

        public Note Note
        {
            get
            {
                return _Note;
            }
            set
            {
                if (_Note != value)
                {
                    _Note = value;

                    RaisePropertyChanged(nameof(Note));
                }
            }
        }
    }
}
