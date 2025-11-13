using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
            set { _fileType = value; OnProperttychanged(); LoadFolderContents(); }
        }

        public ObservableCollection<FileItem> Files { get; set; } = new();

        public ICommand SelectFolderCommand { get; }
        public ICommand MoveFilesCommand { get; }

        public MainViewModel()
        {
            SelectFolderCommand = new RelayCommand(o => SelectFolder());
            MoveFilesCommand = new RelayCommand(o => MoveFiles(), o => Files.Count > 0);
        }

        private void SelectFolder()
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FolderPath = dialog.SelectedPath;
                LoadFolderContents();
            }
        }

        private void LoadFolderContents()
        {
            if (!Directory.Exists(FolderPath)) return;

            Files.Clear();

            foreach (var dir in Directory.GetDirectories(FolderPath))
                Files.Add(new FileItem(dir, true));

            foreach (var file in Directory.GetFiles(FolderPath))
            {
                if (string.IsNullOrEmpty(FileType) || Path.GetExtension(file).Equals(FileType, StringComparison.OrdinalIgnoreCase))
                    Files.Add(new FileItem(file));
            }
        }

        private void MoveFiles()
        {
            if (!Directory.Exists(FolderPath)) return;

            foreach (var fileItem in Files.Where(f => !f.IsFolder).ToList())
            {
                if (!string.IsNullOrEmpty(FileType) && !Path.GetExtension(fileItem.Path).Equals(FileType, StringComparison.OrdinalIgnoreCase))
                    continue;

                var extension = Path.GetExtension(fileItem.Path).TrimStart('.').ToUpper();
                if (string.IsNullOrEmpty(extension))
                    extension = "OTHER";

                var targetDir = Path.Combine(FolderPath, extension);
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);

                var targetPath = Path.Combine(targetDir, Path.GetFileName(fileItem.Path));
                if (!File.Exists(targetPath))
                    File.Move(fileItem.Path, targetPath);
            }

            LoadFolderContents();
        }
    }
}
