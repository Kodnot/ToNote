namespace ToNote.Models
{
    public class Todo : BaseModel
    {
        public Todo()
        {
            
        }

        public string FileName { get; set; }

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

        private int _Index;

        public int Index
        {
            get => _Index;
            set
            {
                if (_Index != value)
                {
                    _Index = value;

                    RaisePropertyChanged(nameof(Index));
                }
            }
        }
    }
}
