namespace DownloaderUI.Models
{
    public class DownloadFileAddedMessage(DownloadItem downloadItem)
    {
        public DownloadItem DownloadItem { get; } = downloadItem;
    }
}
