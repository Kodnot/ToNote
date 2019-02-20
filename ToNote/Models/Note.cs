namespace ToNote.Models
{
    using System.IO;

    public class Note : BaseModel
    {
        public Note(string name)
        {
            Name = name;
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
    }
}
