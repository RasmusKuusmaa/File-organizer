using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using FileOrganizer.helpers;
using System.Collections.ObjectModel;
using System.IO;
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
        public ObservableCollection<string> Files { get; set; } = new ObservableCollection<string>();
        public ICommand SelectFolderCommand { get; }
        private void SelectFolder()
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FolderPath = dialog.SelectedPath;

                Files.Clear();
                foreach (var file in Directory.GetFiles(FolderPath))
                {
                    Files.Add(Path.GetFileName(file));
                }
            }
        }

        public MainViewmodel()
        {
            SelectFolderCommand = new RelayCommand(o => SelectFolder());
        }
    }
}
