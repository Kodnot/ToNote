using System.Collections.Generic;
using System.Windows.Documents;
using Newtonsoft.Json;

namespace ToNote.Models
{
    using System.IO;

    public class Note : BaseModel
    {
        public Note(string name, string description = null)
        {
            Name = name;
            Description = description;
            FileNames = new List<string>();
        }

        private string _Name;

        public string Name
        {
            get => _Name;
            set
            {
                if (_Name != value)
                {
                    _Name = value;

                    RaisePropertyChanged(nameof(Name));
                }
            }
        }

        private string _Description;

        public string Description
        {
            get => _Description;
            set
            {
                if (_Description != value)
                {
                    _Description = value;

                    RaisePropertyChanged(nameof(Description));
                }
            }
        }

        private List<string> _FileNames;

        public List<string> FileNames
        {
            get => _FileNames;
            set
            {
                if (_FileNames != value)
                {
                    _FileNames = value;

                    RaisePropertyChanged(nameof(FileNames));
                }
            }
        }

        public void DeleteFile(string file)
        {
            if (File.Exists(file))
                File.Delete(file);

            if (this.FileNames.Contains(file))
                this.FileNames.Remove(file);
        }
    }
}
