using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using FileOrganizer.helpers;
namespace FileOrganizer.ViewModels
{
    public class MainViewmodel : BaseViewModel
    {
        private string _folderPath;

        public string FolderPath
        {
            get { return _folderPath; }
            set { _folderPath = value;
                OnProperttychanged();
            }
        }
        public ICommand SelectFolderCommand { get; }
        private void SelectFolder()
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                FolderPath = dialog.SelectedPath;
            }

        }
        public MainViewmodel()
        {
            SelectFolderCommand = new RelayCommand(o => SelectFolder());
        }
    }
}
