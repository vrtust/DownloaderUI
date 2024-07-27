using Downloader;
using System;

namespace DownloaderUI.Models
{
    public class DownloadItemInfo
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public double ProgressPercentage { get; set; }
        public string FileSize { get; set; }
        public string FolderPath { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public DownloadPackage Pack { get; set; }
        public string PackageJson { get; set; }
        public string ExMessage { get; set; }
        public bool IsOpen { get; set; }
        public bool IsOpenFolder { get; set; }
        public string BytesPerSecondSpeed { get; set; }
        public string ReceivedBytesSize { get; set; }
    }
}
