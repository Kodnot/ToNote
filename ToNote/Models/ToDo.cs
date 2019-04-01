using System;

namespace ToNote.Models
{
    public class Todo : BaseModel
    {
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

                    if (_IsChecked == true)
                    {
                        _LastDone = SelectedDate;
                        _SelectedDate = null;
                    }

                    RaisePropertyChanged(nameof(SelectedDate));
                    RaisePropertyChanged(nameof(IsDatePast));
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

        private DateTime? _SelectedDate;

        public DateTime? SelectedDate
        {
            get => _SelectedDate;
            set
            {
                if (_SelectedDate != value)
                {
                    _SelectedDate = value;

                    RaisePropertyChanged(nameof(SelectedDate));
                    RaisePropertyChanged(nameof(IsDatePast));
                }
                
            }
        }

        private DateTime? _LastDone;

        public DateTime? LastDone
        {
            get => _LastDone;
        }

        public bool IsDatePast => _SelectedDate != null ? DateTime.Compare((DateTime)_SelectedDate, DateTime.Today) > 0 ? true : false : false;
    }
}
