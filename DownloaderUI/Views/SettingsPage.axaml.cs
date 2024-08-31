using Avalonia.Controls;
using DownloaderUI.ViewModels;


namespace DownloaderUI;

public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        InitializeComponent();

        DataContext = new SettingsPageViewModel();
    }
}