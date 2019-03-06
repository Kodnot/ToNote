using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToNote.Models
{
    class TodoNote : BaseModel
    {
        
        public TodoNote()
        {
            
        }

        string _todoText;

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

        public string TodoText
        {
            get
            {
                return _todoText;
            }
            set
            {
                _todoText = value;
                RaisePropertyChanged(nameof(TodoText));
            }
        }
    }
}
