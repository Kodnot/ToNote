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
using ToNote.ViewModels;

namespace ToNote.ViewModels
{
    class SettingsViewModel : BaseDialogViewModel<int>
    {
        public ICommand ImportCommand { get; set;}
        public ICommand ExportCommand { get; set;}

        public SettingsViewModel()
        {

        }
    }
}
