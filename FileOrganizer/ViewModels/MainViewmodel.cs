using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Forms;
using FileOrganizer.Helpers;
using FileOrganizer.Models;

namespace FileOrganizer.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _folderPath;
        public string FolderPath
        {
            get => _folderPath;
            set { _folderPath = value; OnPropertyChanged(); }
        }

        private bool _organizeAll;
        public bool OrganizeAll
        {
            get => _organizeAll;
            set { _organizeAll = value; OnPropertyChanged(); LoadFolderContents(); }
        }

        public ObservableCollection<FileItem> Files { get; set; } = new();
        public ObservableCollection<string> FileTypes { get; set; } = new() { ".txt", ".pdf", ".docx", ".xlsx", ".jpg", ".png" };
        public ObservableCollection<string> SelectedFileTypes { get; set; } = new();

        private string _customFileType;
        public string CustomFileType
        {
            get => _customFileType;
            set { _customFileType = value; OnPropertyChanged(); }
        }

        public ICommand SelectFolderCommand { get; }
        public ICommand AddCustomFileTypeCommand { get; }
        public ICommand MoveFilesCommand { get; }

        public MainViewModel()
        {
            SelectFolderCommand = new RelayCommand(o => SelectFolder());
            AddCustomFileTypeCommand = new RelayCommand(o => AddCustomFileType());
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
        public void UpdateSelectedFileTypes(System.Collections.IList selected)
        {
            SelectedFileTypes.Clear();
            foreach (var item in selected)
                SelectedFileTypes.Add(item.ToString());
            LoadFolderContents();
        }

        private void LoadFolderContents()
        {
            if (!Directory.Exists(FolderPath)) return;

            Files.Clear();

            foreach (var dir in Directory.GetDirectories(FolderPath))
                Files.Add(new FileItem(dir, true));

            foreach (var file in Directory.GetFiles(FolderPath))
            {
                Files.Add(new FileItem(file));
            }
        }

        private void AddCustomFileType()
        {
            if (!string.IsNullOrEmpty(CustomFileType) && !SelectedFileTypes.Contains(CustomFileType))
            {
                SelectedFileTypes.Add(CustomFileType.StartsWith(".") ? CustomFileType : "." + CustomFileType);
                CustomFileType = string.Empty;
                LoadFolderContents();
            }
        }

        private void MoveFiles()
        {
            if (!Directory.Exists(FolderPath)) return;

            foreach (var fileItem in Files.Where(f => !f.IsFolder).ToList())
            {
                if (!OrganizeAll && !SelectedFileTypes.Any(t => Path.GetExtension(fileItem.Path).Equals(t, StringComparison.OrdinalIgnoreCase)))
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
