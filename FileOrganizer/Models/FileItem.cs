using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Drawing; // <- IMPORTANT

namespace FileOrganizer.Models
{
    public class FileItem
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public ImageSource Icon { get; set; }

        public FileItem(string path)
        {
            FilePath = path;
            FileName = Path.GetFileName(path);
            Icon = GetIcon(path);
        }
        private ImageSource GetIcon(string filePath)
        {
            try
            {
                System.Drawing.Icon sysIcon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
                return Imaging.CreateBitmapSourceFromHIcon(
                    sysIcon.Handle,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(16, 16));
            }
            catch
            {
                return null;
            }
        }

    }
}
