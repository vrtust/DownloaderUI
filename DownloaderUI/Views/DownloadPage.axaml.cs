using Avalonia.Controls;
using DownloaderUI.ViewModels;

namespace DownloaderUI.Views;

public partial class DownloadPage : UserControl
{
    public DownloadPage()
    {
        InitializeComponent();
        DataContext = new DownloadPageViewModel();
    }
}