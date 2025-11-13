using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Drawing; // <- IMPORTANT

namespace FileOrganizer.Models
{
    using System;
    using System.IO;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Interop;

    namespace FileOrganizer.Models
    {
        public class FileItem
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public ImageSource Icon { get; set; }
            public bool IsFolder { get; set; }

            public FileItem(string path, bool isFolder = false)
            {
                Path = path;
                Name = System.IO.Path.GetFileName(path);
                IsFolder = isFolder;
                Icon = GetIcon(path, isFolder);
            }

            private ImageSource GetIcon(string path, bool isFolder)
            {
                try
                {
                    System.Drawing.Icon sysIcon;

                    if (isFolder)
                    {
                        sysIcon = System.Drawing.SystemIcons.WinLogo;
                                                                      
                    }
                    else
                    {
                        sysIcon = System.Drawing.Icon.ExtractAssociatedIcon(path);
                    }

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

}
