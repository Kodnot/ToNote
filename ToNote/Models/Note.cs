namespace ToNote.Models
{
    using System.Collections.Generic;
    using System.IO;

    public class Note : BaseModel
    {
        public Note(string name, string description = null)
        {
            Name = name;
            Description = description;
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
            get => _FileNames ?? (_FileNames = new List<string>());
            set
            {
                if (_FileNames != value)
                {
                    _FileNames = value;

                    RaisePropertyChanged(nameof(FileNames));
                }
            }
        }

        private List<Todo> _Todos;

        public List<Todo> Todos
        {
            get => _Todos ?? (_Todos = new List<Todo>());
            set
            {
                if (_Todos != value)
                {
                    _Todos = value;

                    RaisePropertyChanged(nameof(Todos));
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

        public void RemoveTodo(Todo todo)
        {
            if (File.Exists(todo.FileName))
                File.Delete(todo.FileName);

            if (this.Todos.Contains(todo))
                this.Todos.Remove(todo);
        }
    }
}
