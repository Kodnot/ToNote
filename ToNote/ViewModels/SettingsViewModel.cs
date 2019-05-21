using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using ToNote.Logic;
using ToNote.Models;

namespace ToNote.ViewModels
{
    class SettingsViewModel : BaseDialogViewModel<bool>
    {
        private ObservableCollection<Note> _Notes;

        public ObservableCollection<Note> Notes
        {
            get
            {
                return _Notes ?? (_Notes = new ObservableCollection<Note>());
            }
            set
            {
                if (_Notes != value)
                {
                    _Notes = value;

                    RaisePropertyChanged(nameof(Notes));
                }
            }
        }

        public ICollectionView FilteredNotes => CollectionViewSource.GetDefaultView(Notes);

        public SettingsViewModel()
        {

        }

        private ICommand _ExportCommand;

        public ICommand ExportCommand
        {
            get => _ExportCommand ?? (_ExportCommand = new RelayCommand(() =>
            {
                IOHandler.ExportNotes(Notes);
            }));
        }

        private ICommand _ImportCommand;

        public ICommand ImportCommand
        {
            get => _ImportCommand ?? (_ImportCommand = new RelayCommand(() =>
            {
                var notes = IOHandler.ReadNotesFromFile();

                if (notes != null)
                    Notes = new ObservableCollection<Note>(notes);

                RaisePropertyChanged(nameof(FilteredNotes));
            }));
        }
    }
}
