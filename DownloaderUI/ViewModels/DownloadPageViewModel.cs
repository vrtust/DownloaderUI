using Avalonia.Collections;
using Avalonia.Controls;
using DownloaderUI.Models;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using System;
using System.IO;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;

namespace DownloaderUI.ViewModels
{
    public class DownloadPageViewModel : ViewModelBase
    {
        private readonly AvaloniaList<double> _chunkProgressCollection = [];
        public AvaloniaList<double> ChunkProgressCollection => _chunkProgressCollection;
        public void UpdateChunkProgress(int chunkIndex, double progress)
        {
            if (chunkIndex >= 0 && chunkIndex < _chunkProgressCollection.Count)
            {
                _chunkProgressCollection[chunkIndex] = progress;
            }
        }


        private string _selectedFolder;
        public string SelectedFolder
        {
            get => _selectedFolder;
            set => this.RaiseAndSetIfChanged(ref _selectedFolder, value);
        }

        private string _selectedFolderPath;
        public string SelectedFolderPath
        {
            get => _selectedFolderPath;
            set => this.RaiseAndSetIfChanged(ref _selectedFolderPath, value);
        }

        private string _url;
        public string Url
        {
            get => _url;
            set => this.RaiseAndSetIfChanged(ref _url, value);
        }

        private string _fileSize;
        public string FileSize
        {
            get => _fileSize;
            set => this.RaiseAndSetIfChanged(ref _fileSize, value);
        }

        private long _orginalFileSize;
        public long OrginalFileSize
        {
            get => _orginalFileSize;
            set => this.RaiseAndSetIfChanged(ref _orginalFileSize, value);
        }

        private string _fileName;
        public string FileName
        {
            get => _fileName;
            set => this.RaiseAndSetIfChanged(ref _fileName, value);
        }

        private string _currentMessage;

        public string CurrentMessage
        {
            get => _currentMessage;
            set => this.RaiseAndSetIfChanged(ref _currentMessage, value);
        }

        private string _freeSpace;

        public string FreeSpace
        {
            get => _freeSpace;
            set => this.RaiseAndSetIfChanged(ref _freeSpace, value);
        }

        private bool _praseUrlEnable;

        public bool PraseUrlEnable
        {
            get => _praseUrlEnable;
            set => this.RaiseAndSetIfChanged(ref _praseUrlEnable, value);
        }

        private bool _isParsed;

        public bool IsParsed
        {
            get => _isParsed;
            set => this.RaiseAndSetIfChanged(ref _isParsed, value);
        }

        private DownloadItem _downloadItem;

        public DownloadItem DownloadItem
        {
            get => _downloadItem;
            set => this.RaiseAndSetIfChanged(ref _downloadItem, value);
        }

        private bool _selected;
        public bool Selected
        {
            get => _selected;
            set => this.RaiseAndSetIfChanged(ref _selected, value);
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

        public ReactiveCommand<Unit, Unit> SelectFolderCommand { get; }
        public ReactiveCommand<Unit, Unit> ParseUrlCommand { get; }
        public ReactiveCommand<Unit, Unit> DownloadUrlCommand { get; }

        public DownloadPageViewModel()
        {
            SelectFolderCommand = ReactiveCommand.CreateFromTask(SelectFolder);
            ParseUrlCommand = ReactiveCommand.Create(ParseUrl);
            DownloadUrlCommand = ReactiveCommand.Create(DownloadUrl);

            DownloadItem = new DownloadItem();

            Selected = false;

            IsParsed = false;
            PraseUrlEnable = false;
        }

        private async Task SelectFolder()
        {
            var dialog = new OpenFolderDialog();
            var result = await dialog.ShowAsync(App.MainWindow);

            if (result != null)
            {
                Selected = true;

                PraseUrlEnable = true;

                SelectedFolder = result;
                _ = new DirectoryInfo(SelectedFolder);

                // DownloadItem.FolderPath = path;
                DownloadItem.FolderPath = SelectedFolder;

                string root = Path.GetPathRoot(SelectedFolder);

                DriveInfo drive = new(root);
                FreeSpace += FormatBytes(drive.AvailableFreeSpace);
                try
                {
                    if (drive.AvailableFreeSpace < OrginalFileSize)
                    {
                        FreeSpace += ". No enough space here! Please select another folder.";
                        PraseUrlEnable = false;
                    }
                }
                catch (Exception)
                {

                }

            }
        }

        private async void ParseUrl()
        {
            FileSize = "";
            FileName = "";

            DownloadItem.IsOpen = IsOpen;
            DownloadItem.IsOpenFolder = IsOpenFolder;

            DownloadItem.Url = Url;
            if (!Uri.TryCreate(Url, UriKind.Absolute, out _))
            {
                CurrentMessage = "invalid url";
            }
            else
            {
                using HttpClient httpClient = new();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
                try
                {
                    HttpResponseMessage response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, Url));

                    if (response.IsSuccessStatusCode)
                    {
                        DownloadItem.Id = Guid.NewGuid();
                        string fileName = GetFileNameFromUrl(Url);
                        FileName = fileName;
                        DownloadItem.FileName = fileName;

                        // xxxx B
                        long fileSize = response.Content.Headers.ContentLength ?? -1;

                        if (fileSize > 0)
                        {
                            OrginalFileSize = fileSize;
                            FileSize = FormatBytes(fileSize);
                            DownloadItem.FileSize = FormatBytes(fileSize);
                        }
                        else
                        {
                            FileSize = "Failed to retrieve file size.";
                        }
                        IsParsed = true;
                    }
                    else
                    {
                        CurrentMessage = $"Failed to retrieve information from URL. Status code: {response.StatusCode}";

                        var dialog = new ContentDialog()
                        {
                            Title = $"Failed to retrieve information from URL. Status code: {response.StatusCode}",
                            PrimaryButtonText = "Ok",
                            CloseButtonText = "Close"
                        };

                        var result = await dialog.ShowAsync();
                    }
                }
                catch (Exception ex)
                {
                    CurrentMessage = $"An error occurred: {ex.Message}";

                    var dialog = new ContentDialog()
                    {
                        Title = $"An error occurred: {ex.Message}",
                        PrimaryButtonText = "Ok",
                        CloseButtonText = "Close"
                    };

                    var result = await dialog.ShowAsync();
                }
            }
        }

        private async void DownloadUrl()
        {
            if (!IsParsed)
            {
                ParseUrl();
            }
            MessageBus.Current.SendMessage(new DownloadFileAddedMessage(DownloadItem));
        }

        static string FormatBytes(long bytes)
        {
            string[] suffixes = ["B", "KB", "MB", "GB", "TB"];
            int suffixIndex = 0;
            double formattedValue = bytes;

            while (formattedValue >= 1024 && suffixIndex < suffixes.Length - 1)
            {
                formattedValue /= 1024;
                suffixIndex++;
            }

            return $"{formattedValue:F3} {suffixes[suffixIndex]}";
        }

        static string GetFileNameFromUrl(string url)
        {
            Uri uri = new(url);
            string fileName = Path.GetFileName(uri.LocalPath);

            return fileName;
        }
    }
}
