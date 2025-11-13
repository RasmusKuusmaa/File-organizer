using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Forms;
using FileOrganizer.helpers;
using FileOrganizer.Models;
using FileOrganizer.helpers;

namespace FileOrganizer.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _folderPath;
        public string FolderPath
        {
            get => _folderPath;
            set { _folderPath = value; OnProperttychanged(); }
        }

        private string _fileType;
        public string FileType
        {
            get => _fileType;
            set { _fileType = value; OnProperttychanged(); LoadFilesByType(); }
        }

        public ObservableCollection<FileItem> Files { get; set; } = new();

        public ICommand SelectFolderCommand { get; }

        public MainViewModel()
        {
            SelectFolderCommand = new RelayCommand(o => SelectFolder());
        }

        private void SelectFolder()
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FolderPath = dialog.SelectedPath;
                LoadFilesByType();
            }
        }

        private void LoadFilesByType()
        {
            if (!Directory.Exists(FolderPath)) return;

            Files.Clear();
            foreach (var file in Directory.GetFiles(FolderPath))
            {
                if (string.IsNullOrEmpty(FileType) || Path.GetExtension(file).Equals(FileType, StringComparison.OrdinalIgnoreCase))
                {
                    Files.Add(new FileItem(file));
                }
            }
        }
    }
}
