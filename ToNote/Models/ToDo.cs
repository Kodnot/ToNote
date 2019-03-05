using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToNote.Models
{
    class ToDo : BaseModel
    {
        
        public ToDo()
        {
            
        }

        string _toDoText;

        private bool _isChecked = false;

        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    RaisePropertyChanged(nameof(IsChecked));
                }
            }
        }

        public string ToDoText
        {
            get
            {
                return _toDoText;
            }
            set
            {
                _toDoText = value;
                RaisePropertyChanged(nameof(ToDoText));
            }
        }
    }
}
