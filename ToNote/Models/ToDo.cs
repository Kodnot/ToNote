namespace ToNote.Models
{
    public class Todo : BaseModel
    {
        public Todo()
        {
            
        }

        private bool _IsChecked = false;

        public bool IsChecked
        {
            get => _IsChecked;
            set
            {
                if (_IsChecked != value)
                {
                    _IsChecked = value;

                    RaisePropertyChanged(nameof(IsChecked));
                }
            }
        }
    }
}
