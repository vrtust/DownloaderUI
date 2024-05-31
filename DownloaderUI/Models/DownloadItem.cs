using Downloader;
using ReactiveUI;
using System;

namespace DownloaderUI.Models
{
    public class DownloadItem : ReactiveObject
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }

        private double _progressPercentage;

        public double ProgressPercentage
        {
            get => _progressPercentage;
            set => this.RaiseAndSetIfChanged(ref _progressPercentage, value);
        }
        public string FileSize { get; set; }
        public string FolderPath { get; set; }

        public string Path { get; set; }

        public string Url { get; set; }

        private string _status;

        public string Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }

        public DownloadPackage Pack { get; set; }

        public string PackageJson { get; set; }

        private string _exMessage;

        public string ExMessage
        {
            get => _exMessage;
            set => this.RaiseAndSetIfChanged(ref _exMessage, value);
        }

        private bool _isOpen;

        public bool IsOpen
        {
            get => _isOpen;
            set => this.RaiseAndSetIfChanged(ref _isOpen, value);
        }

        private bool _isOpenFolder;

        public bool IsOpenFolder
        {
            get => _isOpenFolder;
            set => this.RaiseAndSetIfChanged(ref _isOpenFolder, value);
        }

        private string _bytesPerSecondSpeed;

        public string BytesPerSecondSpeed
        {
            get => _bytesPerSecondSpeed;
            set => this.RaiseAndSetIfChanged(ref _bytesPerSecondSpeed, value);
        }

        private string _receivedBytesSize;

        public string ReceivedBytesSize
        {
            get => _receivedBytesSize;
            set => this.RaiseAndSetIfChanged(ref _receivedBytesSize, value);
        }
    }
}
