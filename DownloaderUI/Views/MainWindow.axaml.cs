using Avalonia.Controls;
using DownloaderUI.ViewModels;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;

namespace DownloaderUI.Views
{
    public partial class MainWindow : Window
    {
        public SettingsPageViewModel settingsPageViewModel;
        public DownloadListPageViewModel downloadListPageViewModel;
        private Dictionary<string, object> pagePool;

        public MainWindow()
        {
            InitializeComponent();

            settingsPageViewModel = new SettingsPageViewModel();
            downloadListPageViewModel = new DownloadListPageViewModel();
            pagePool = new Dictionary<string, object>();

            // Default NavView
            var nv = this.FindControl<NavigationView>("MainNavigation");
            nv.SelectionChanged += OnNVSample1SelectionChanged;
            nv.SelectedItem = nv.MenuItems.ElementAt(0);
        }

        private void OnNVSample1SelectionChanged(object sender, NavigationViewSelectionChangedEventArgs e)
        {
            object? pageInstance = null;
            if (e.IsSettingsSelected)
            {
                if (!pagePool.TryGetValue(nameof(SettingsPage), out pageInstance))
                {
                    pageInstance = new SettingsPage();
                    pagePool.Add(nameof(SettingsPage), pageInstance);
                }

                (sender as NavigationView).DataContext = settingsPageViewModel;
            }
            else if (e.SelectedItem is NavigationViewItem nvi)
            {
                var smpPageType = $"DownloaderUI.Views.{nvi.Tag}";
                if (!pagePool.TryGetValue(smpPageType, out pageInstance))
                {
                    pageInstance = Activator.CreateInstance(Type.GetType(smpPageType));
                    pagePool.Add(smpPageType, pageInstance);
                }

                var viewModel = pageInstance switch
                {
                    DownloadListPage => downloadListPageViewModel,
                    _ => null
                };
                (sender as NavigationView).DataContext = viewModel;
            }
            (sender as NavigationView).Content = pageInstance;
        }
    }
}