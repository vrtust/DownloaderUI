using Avalonia.Controls;
using DownloaderUI.ViewModels;

namespace DownloaderUI.Views;

public partial class DownloadListPage : UserControl
{
    public DownloadListPage()
    {
        InitializeComponent();

        DataContext = new DownloadListPageViewModel();
    }
}