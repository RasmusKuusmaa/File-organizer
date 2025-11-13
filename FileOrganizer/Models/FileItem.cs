using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Drawing;
using System.Runtime.InteropServices;

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
            Icon sysIcon;
            if (isFolder)
                sysIcon = GetFolderIcon(path);
            else
                sysIcon = System.Drawing.Icon.ExtractAssociatedIcon(path);

            return Imaging.CreateBitmapSourceFromHIcon(
                sysIcon.Handle,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(16, 16));
        }

        private Icon GetFolderIcon(string folderPath)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            IntPtr hImg = SHGetFileInfo(
                folderPath,
                FILE_ATTRIBUTE_DIRECTORY,
                ref shinfo,
                (uint)Marshal.SizeOf(shinfo),
                SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES);

            Icon icon = (Icon)System.Drawing.Icon.FromHandle(shinfo.hIcon).Clone();
            DestroyIcon(shinfo.hIcon);
            return icon;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_SMALLICON = 0x1;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x10;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbFileInfo,
            uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);
    }
}
