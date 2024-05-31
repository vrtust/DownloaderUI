using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Fonts;
using Avalonia.Styling;
using Avalonia.Threading;
using DownloaderUI.Models;
using DownloaderUI.Service;
using FluentAvalonia.Styling;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DownloaderUI.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public SettingsPageViewModel()
        {
            SelectFolderCommand = ReactiveCommand.CreateFromTask(SelectFolder);

            Load();
            GetPredefColors();
            _faTheme = App.Current.Styles[0] as FluentAvaloniaTheme;

            if(!string.IsNullOrEmpty(DefaultPath))
            {
                Selected = true;
                GetFreeSpace();
            }

            this.WhenAnyValue(vm => vm.CurrentAppTheme)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(value =>
                {
                    Dispatcher.UIThread.InvokeAsync((Action)(async () =>
                    {
                        try
                        {
                            var newTheme = GetThemeVariant(value);
                            if (newTheme != null)
                            {
                                Application.Current.RequestedThemeVariant = newTheme;
                            }
                            if (value != _system)
                            {
                                _faTheme.PreferSystemTheme = false;
                            }
                            else
                            {
                                _faTheme.PreferSystemTheme = true;
                            }
                            await SaveAsync();
                        }
                        catch (Exception ex)
                        {

                        }

                    }));

                });

            this.WhenAnyValue(vm => vm.UseCustomAccent)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(value =>
                {
                    Dispatcher.UIThread.InvokeAsync((Action)(async () =>
                    {
                        try
                        {
                            if (value)
                            {
                                if (_faTheme.TryGetResource("SystemAccentColor", null, out var curColor))
                                {
                                    _customAccentColor = (Color)curColor;
                                    _listBoxColor = _customAccentColor;
                                }
                                else
                                {
                                    // This should never happen, if it does, something bad has happened
                                    throw new Exception("Unable to retreive SystemAccentColor");
                                }
                            }
                            else
                            {
                                // Restore system color
                                _customAccentColor = default;
                                _listBoxColor = default;
                                UpdateAppAccentColor(null);
                            }
                            await SaveAsync();
                        }
                        catch (Exception ex)
                        {

                        }

                    }));

                });

            this.WhenAnyValue(vm => vm.ListBoxColor)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(value =>
                {
                    Dispatcher.UIThread.InvokeAsync((Action)(async () =>
                    {

                        try
                        {
                            if (value != null && UseCustomAccent)
                            {
                                _customAccentColor = ((Color)value);
                                UpdateAppAccentColor(((Color)value));
                            }
                            await SaveAsync();
                        }
                        catch (Exception ex)
                        {

                        }

                    }));

                });

            this.WhenAnyValue(vm => vm.CustomAccentColor)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(value =>
                {
                    Dispatcher.UIThread.InvokeAsync((Action)(async () =>
                    {

                        try
                        {
                            if (UseCustomAccent)
                            {
                                _listBoxColor = value;
                                UpdateAppAccentColor(value);
                            }
                            await SaveAsync();
                        }
                        catch (Exception ex)
                        {

                        }

                    }));

                });

            this.WhenAnyValue(vm => vm.FontSize)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(value =>
                {
                    Dispatcher.UIThread.InvokeAsync((Action)(async () =>
                    {
                        try
                        {
                            if (value != null)
                            {
                                Application.Current.Resources["ControlContentThemeFontSize"] = FontSize;
                            }
                            await SaveAsync();
                        }
                        catch (Exception ex)
                        {

                        }

                    }));

                });

            this.WhenAnyValue(vm => vm.CurrentFont)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(value =>
                {
                    Dispatcher.UIThread.InvokeAsync((Action)(async () =>
                    {
                        try
                        {
                            if (value != null)
                            {
                                Application.Current.Resources["ContentControlThemeFontFamily"] = CurrentFont;
                            }
                            await SaveAsync();
                        }
                        catch (Exception ex)
                        {

                        }

                    }));

                });

            this.WhenAnyValue(vm => vm.CurrentLanguage)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(value =>
                {
                    Dispatcher.UIThread.InvokeAsync((Action)(async () =>
                    {
                        try
                        {
                            string SystemLanguage = CultureInfo.CurrentCulture.Name;
                            if (value.Equals("System"))
                            {
                                value = SystemLanguage;
                            }
                            if (LanguageList.Contains(value))
                            {
                                TranslateService.Translate(value);
                            }
                            else
                            {
                                TranslateService.Translate("en-US");
                            }
                            await SaveAsync();
                        }
                        catch (Exception ex)
                        {

                        }

                    }));

                });

            // Combine monitoring of all property changes
            var propertyChanges = new[]
            {
                this.WhenAnyValue(x => x.DefaultPath).Select(_ => Unit.Default),
                this.WhenAnyValue(x => x.BufferBlockSize).Select(_ => Unit.Default),
                this.WhenAnyValue(x => x.ChunkCount).Select(_ => Unit.Default),
                this.WhenAnyValue(x => x.MaximumBytesPerSecond).Select(_ => Unit.Default),
                this.WhenAnyValue(x => x.UnitForMaximumBytesPerSecond).Select(_ => Unit.Default),
                this.WhenAnyValue(x => x.MaxTryAgainOnFailover).Select(_ => Unit.Default),
                this.WhenAnyValue(x => x.MaximumMemoryBufferBytes).Select(_ => Unit.Default),
                this.WhenAnyValue(x => x.UnitForMaximumMemoryBufferBytes).Select(_ => Unit.Default),
                this.WhenAnyValue(x => x.ParallelDownload).Select(_ => Unit.Default),
                this.WhenAnyValue(x => x.ParallelCount).Select(_ => Unit.Default),
                this.WhenAnyValue(x => x.Timeout).Select(_ => Unit.Default),
                this.WhenAnyValue(x => x.RangeDownload).Select(_ => Unit.Default),
                this.WhenAnyValue(x => x.RangeLow).Select(_ => Unit.Default),
                this.WhenAnyValue(x => x.RangeHigh).Select(_ => Unit.Default),
                this.WhenAnyValue(x => x.ClearPackageOnCompletionWithFailure).Select(_ => Unit.Default),
                this.WhenAnyValue(x => x.MinimumSizeOfChunking).Select(_ => Unit.Default),
                this.WhenAnyValue(x => x.ReserveStorageSpaceBeforeStartingDownload).Select(_ => Unit.Default),
                this.WhenAnyValue(x => x.UserAgent).Select(_ => Unit.Default),
                this.WhenAnyValue(x => x.ProxyUri).Select(_ => Unit.Default)
            };

            var mergedPropertyChanges = propertyChanges.Merge();

            mergedPropertyChanges
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(_ =>
                {
                    SaveAsync();
                    SyncDwonloadSettingsAsync();
                });
        }

        public ReactiveCommand<Unit, Unit> SelectFolderCommand { get; }

        private async Task SelectFolder()
        {
            var dialog = new OpenFolderDialog();
            var result = await dialog.ShowAsync(App.MainWindow);

            if (result != null)
            {
                DefaultPath = result;
                Selected = true;
                GetFreeSpace();
            }
        }

        private IFontCollection _fontsList = FontManager.Current.SystemFonts;

        public IFontCollection FontsList
        {
            get => _fontsList;
            set => this.RaiseAndSetIfChanged(ref _fontsList, value);
        }

        private FontFamily _currentFont;

        public FontFamily CurrentFont
        {
            get => _currentFont;
            set => this.RaiseAndSetIfChanged(ref _currentFont, value);
        }

        private int _fontSize = 15;

        public int FontSize
        {
            get => _fontSize;
            set => this.RaiseAndSetIfChanged(ref _fontSize, value);
        }

        private string[] _appThemes = [_system, _light, _dark /*, FluentAvaloniaTheme.HighContrastTheme*/];

        public string[] AppThemes
        {
            get => _appThemes;
            set => this.RaiseAndSetIfChanged(ref _appThemes, value);
        }

        private string _currentAppTheme = _system;

        public string CurrentAppTheme
        {
            get => _currentAppTheme;
            set => this.RaiseAndSetIfChanged(ref _currentAppTheme, value);
        }

        private string[] _languageList = ["System", "en-US", "zh-CN"];

        public string[] LanguageList
        {
            get => _languageList;
            set => this.RaiseAndSetIfChanged(ref _languageList, value);
        }

        private string _currentLanguage = "System";

        public string CurrentLanguage
        {
            get => _currentLanguage;
            set => this.RaiseAndSetIfChanged(ref _currentLanguage, value);
        }

        private static ThemeVariant? GetThemeVariant(string value)
        {
            return value switch
            {
                _light => ThemeVariant.Light,
                _dark => ThemeVariant.Dark,
                _ => null,
            };
        }

        private bool _useCustomAccentColor;

        public bool UseCustomAccent
        {
            get => _useCustomAccentColor;
            set => this.RaiseAndSetIfChanged(ref _useCustomAccentColor, value);
        }

        private Color? _listBoxColor;

        public Color? ListBoxColor
        {
            get => _listBoxColor;
            set => this.RaiseAndSetIfChanged(ref _listBoxColor, value);
        }

        private Color _customAccentColor = Colors.SlateBlue;

        public Color CustomAccentColor
        {
            get => _customAccentColor;
            set => this.RaiseAndSetIfChanged(ref _customAccentColor, value);
        }

        private string? _defaultPath;

        public string? DefaultPath
        {
            get => _defaultPath;
            set => this.RaiseAndSetIfChanged(ref _defaultPath, value);
        }

        private bool _selected;
        public bool Selected
        {
            get => _selected;
            set => this.RaiseAndSetIfChanged(ref _selected, value);
        }

        public void GetFreeSpace()
        {
            string root = Path.GetPathRoot(DefaultPath);

            DriveInfo drive = new(root);
            FreeSpace = FormatBytes(drive.AvailableFreeSpace);
        }

        private string _freeSpace;

        public string FreeSpace
        {
            get => _freeSpace;
            set => this.RaiseAndSetIfChanged(ref _freeSpace, value);
        }

        public static string FormatBytes(long bytes)
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

        // See more in
        // https://github.com/bezzad/Downloader#complex-configuration

        private int _bufferBlockSize = 1024;

        public int BufferBlockSize
        {
            get => _bufferBlockSize;
            set
            {
                this.RaiseAndSetIfChanged(ref _bufferBlockSize, value);
                DownloadSettings.Instance.BufferBlockSize = value;
            }
        }

        private int _chunkCount = 8;

        public int ChunkCount
        {
            get => _chunkCount;
            set
            {
                this.RaiseAndSetIfChanged(ref _chunkCount, value);
                DownloadSettings.Instance.ChunkCount = value;
            }
        }

        private int _maximumBytesPerSecond;

        public int MaximumBytesPerSecond
        {
            get => _maximumBytesPerSecond;
            set => this.RaiseAndSetIfChanged(ref _maximumBytesPerSecond, value);
        }

        public List<string> UnitList { get; } = ["B", "KB", "MB", "GB"];

        private string _unitForMaximumBytesPerSecond = "B";

        public string UnitForMaximumBytesPerSecond
        {
            get => _unitForMaximumBytesPerSecond;
            set => this.RaiseAndSetIfChanged(ref _unitForMaximumBytesPerSecond, value);
        }

        private int _maxTryAgainOnFailover = 5;

        public int MaxTryAgainOnFailover
        {
            get => _maxTryAgainOnFailover;
            set => this.RaiseAndSetIfChanged(ref _maxTryAgainOnFailover, value);
        }

        private int _maximumMemoryBufferBytes;

        public int MaximumMemoryBufferBytes
        {
            get => _maximumMemoryBufferBytes;
            set => this.RaiseAndSetIfChanged(ref _maximumMemoryBufferBytes, value);
        }

        private string _unitForMaximumMemoryBufferBytes = "B";

        public string UnitForMaximumMemoryBufferBytes
        {
            get => _unitForMaximumMemoryBufferBytes;
            set => this.RaiseAndSetIfChanged(ref _unitForMaximumMemoryBufferBytes, value);
        }

        private bool _parallelDownload = true;

        public bool ParallelDownload
        {
            get => _parallelDownload;
            set => this.RaiseAndSetIfChanged(ref _parallelDownload, value);
        }

        public List<bool> TrueOrFalseList { get; } = [true, false];

        private int _parallelCount = 8;

        public int ParallelCount
        {
            get => _parallelCount;
            set => this.RaiseAndSetIfChanged(ref _parallelCount, value);
        }

        private int _timeout = 1000;

        public int Timeout
        {
            get => _timeout;
            set => this.RaiseAndSetIfChanged(ref _timeout, value);
        }

        private bool _rangeDownload;

        public bool RangeDownload
        {
            get => _rangeDownload;
            set => this.RaiseAndSetIfChanged(ref _rangeDownload, value);
        }

        private long _rangeLow;

        public long RangeLow
        {
            get => _rangeLow;
            set => this.RaiseAndSetIfChanged(ref _rangeLow, value);
        }

        private long _rangeHigh;

        public long RangeHigh
        {
            get => _rangeHigh;
            set => this.RaiseAndSetIfChanged(ref _rangeHigh, value);
        }

        private bool _clearPackageOnCompletionWithFailure = true;

        public bool ClearPackageOnCompletionWithFailure
        {
            get => _clearPackageOnCompletionWithFailure;
            set => this.RaiseAndSetIfChanged(ref _clearPackageOnCompletionWithFailure, value);
        }

        private long _minimumSizeOfChunking = 512;

        public long MinimumSizeOfChunking
        {
            get => _minimumSizeOfChunking;
            set => this.RaiseAndSetIfChanged(ref _minimumSizeOfChunking, value);
        }

        private bool _reserveStorageSpaceBeforeStartingDownload;

        public bool ReserveStorageSpaceBeforeStartingDownload
        {
            get => _reserveStorageSpaceBeforeStartingDownload;
            set => this.RaiseAndSetIfChanged(ref _reserveStorageSpaceBeforeStartingDownload, value);
        }

        private string _userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";

        public string UserAgent
        {
            get => _userAgent;
            set => this.RaiseAndSetIfChanged(ref _userAgent, value);
        }

        private string _proxyUri;

        public string ProxyUri
        {
            get => _proxyUri;
            set => this.RaiseAndSetIfChanged(ref _proxyUri, value);
        }

        public List<Color> PredefinedColors { get; private set; }

        private void GetPredefColors()
        {
            PredefinedColors =
            [
                Color.FromRgb(255,185,0),
                Color.FromRgb(255,140,0),
                Color.FromRgb(247,99,12),
                Color.FromRgb(202,80,16),
                Color.FromRgb(218,59,1),
                Color.FromRgb(239,105,80),
                Color.FromRgb(209,52,56),
                Color.FromRgb(255,67,67),
                Color.FromRgb(231,72,86),
                Color.FromRgb(232,17,35),
                Color.FromRgb(234,0,94),
                Color.FromRgb(195,0,82),
                Color.FromRgb(227,0,140),
                Color.FromRgb(191,0,119),
                Color.FromRgb(194,57,179),
                Color.FromRgb(154,0,137),
                Color.FromRgb(0,120,212),
                Color.FromRgb(0,99,177),
                Color.FromRgb(142,140,216),
                Color.FromRgb(107,105,214),
                Color.FromRgb(135,100,184),
                Color.FromRgb(116,77,169),
                Color.FromRgb(177,70,194),
                Color.FromRgb(136,23,152),
                Color.FromRgb(0,153,188),
                Color.FromRgb(45,125,154),
                Color.FromRgb(0,183,195),
                Color.FromRgb(3,131,135),
                Color.FromRgb(0,178,148),
                Color.FromRgb(1,133,116),
                Color.FromRgb(0,204,106),
                Color.FromRgb(16,137,62),
                Color.FromRgb(122,117,116),
                Color.FromRgb(93,90,88),
                Color.FromRgb(104,118,138),
                Color.FromRgb(81,92,107),
                Color.FromRgb(86,124,115),
                Color.FromRgb(72,104,96),
                Color.FromRgb(73,130,5),
                Color.FromRgb(16,124,16),
                Color.FromRgb(118,118,118),
                Color.FromRgb(76,74,72),
                Color.FromRgb(105,121,126),
                Color.FromRgb(74,84,89),
                Color.FromRgb(100,124,100),
                Color.FromRgb(82,94,84),
                Color.FromRgb(132,117,69),
                Color.FromRgb(126,115,95)
            ];
        }

        public class Settings
        {
            public string CurrentFontName { get; set; }
            public int FontSize { get; set; }
            public string CurrentAppTheme { get; set; }
            public bool UseCustomAccent { get; set; }
            public Color? ListBoxColor { get; set; }
            public Color CustomAccentColor { get; set; }
            public string CurrentLanguage { get; set; }
            public string? DefaultPath { get; set; }
            public int BufferBlockSize { get; set; }
            public int ChunkCount { get; set; }
            public int MaximumBytesPerSecond { get; set; }
            public string UnitForMaximumBytesPerSecond { get; set; }
            public int MaxTryAgainOnFailover { get; set; }
            public int MaximumMemoryBufferBytes { get; set; }
            public string UnitForMaximumMemoryBufferBytes { get; set; }
            public bool ParallelDownload { get; set; }
            public int ParallelCount { get; set; }
            public int Timeout { get; set; }
            public bool RangeDownload { get; set; }
            public long RangeLow { get; set; }
            public long RangeHigh { get; set; }
            public bool ClearPackageOnCompletionWithFailure { get; set; }
            public long MinimumSizeOfChunking { get; set; }
            public bool ReserveStorageSpaceBeforeStartingDownload { get; set; }
            public string UserAgent { get; set; }
            public string ProxyUri { get; set; }

        }

        private void UpdateAppAccentColor(Color? color)
        {
            _faTheme.CustomAccentColor = color;
        }

        private bool _ignoreSetListBoxColor = false;

        private const string _system = "System";
        private const string _dark = "Dark";
        private const string _light = "Light";
        private readonly FluentAvaloniaTheme _faTheme;

        public async Task SaveAsync()
        {
            string DataPath;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                DataPath = @".\DownloadData";
            }
            else
            {
                DataPath = @"./DownloadData";
            }

            if (!Directory.Exists("./DownloadData"))
            {
                Directory.CreateDirectory("./DownloadData");
            }

            if (CurrentFont is null)
            {
                CurrentFont = new FontFamily("Default");
            }

            Settings settings = new Settings()
            {
                CurrentFontName = CurrentFont.Name,
                FontSize = FontSize,
                CurrentAppTheme = CurrentAppTheme,
                UseCustomAccent = UseCustomAccent,
                ListBoxColor = ListBoxColor,
                CustomAccentColor = CustomAccentColor,
                CurrentLanguage = CurrentLanguage,
                DefaultPath = DefaultPath,
                BufferBlockSize = BufferBlockSize,
                ChunkCount = ChunkCount,
                MaximumBytesPerSecond = MaximumBytesPerSecond,
                UnitForMaximumBytesPerSecond = UnitForMaximumBytesPerSecond,
                MaxTryAgainOnFailover = MaxTryAgainOnFailover,
                MaximumMemoryBufferBytes = MaximumMemoryBufferBytes,
                UnitForMaximumMemoryBufferBytes = UnitForMaximumMemoryBufferBytes,
                ParallelDownload = ParallelDownload,
                ParallelCount = ParallelCount,
                Timeout = Timeout,
                RangeDownload = RangeDownload,
                RangeLow = RangeLow,
                RangeHigh = RangeHigh,
                ClearPackageOnCompletionWithFailure = ClearPackageOnCompletionWithFailure,
                MinimumSizeOfChunking = MinimumSizeOfChunking,
                ReserveStorageSpaceBeforeStartingDownload = ReserveStorageSpaceBeforeStartingDownload,
                UserAgent = UserAgent,
                ProxyUri = ProxyUri,
            };

            string SettingsJson = JsonConvert.SerializeObject(settings, Formatting.Indented);
            await File.WriteAllTextAsync(Path.Combine(DataPath, "Settings.json"), SettingsJson);
        }

        public void Load()
        {
            string DataPath = @".\DownloadData";

            string SettingsJson = File.ReadAllText(Path.Combine(DataPath, "Settings.json"));

            Settings loadedSettings = JsonConvert.DeserializeObject<Settings>(SettingsJson);

            CurrentFont = new FontFamily(loadedSettings.CurrentFontName);
            FontSize = loadedSettings.FontSize;
            CurrentAppTheme = loadedSettings.CurrentAppTheme;
            UseCustomAccent = loadedSettings.UseCustomAccent;
            ListBoxColor = loadedSettings.ListBoxColor;
            CustomAccentColor = loadedSettings.CustomAccentColor;
            CurrentLanguage = loadedSettings.CurrentLanguage;
            DefaultPath = loadedSettings.DefaultPath;
            BufferBlockSize = loadedSettings.BufferBlockSize;
            ChunkCount = loadedSettings.ChunkCount;
            MaximumBytesPerSecond = loadedSettings.MaximumBytesPerSecond;
            UnitForMaximumBytesPerSecond = loadedSettings.UnitForMaximumBytesPerSecond;
            MaxTryAgainOnFailover = loadedSettings.MaxTryAgainOnFailover;
            MaximumMemoryBufferBytes = loadedSettings.MaximumMemoryBufferBytes;
            UnitForMaximumMemoryBufferBytes = loadedSettings.UnitForMaximumMemoryBufferBytes;
            ParallelDownload = loadedSettings.ParallelDownload;
            ParallelCount = loadedSettings.ParallelCount;
            Timeout = loadedSettings.Timeout;
            RangeDownload = loadedSettings.RangeDownload;
            RangeLow = loadedSettings.RangeLow;
            RangeHigh = loadedSettings.RangeHigh;
            ClearPackageOnCompletionWithFailure = loadedSettings.ClearPackageOnCompletionWithFailure;
            MinimumSizeOfChunking = loadedSettings.MinimumSizeOfChunking;
            ReserveStorageSpaceBeforeStartingDownload = loadedSettings.ReserveStorageSpaceBeforeStartingDownload;
            UserAgent = loadedSettings.UserAgent;
            ProxyUri = loadedSettings.ProxyUri;
        }

        public async Task SyncDwonloadSettingsAsync()
        {
            DownloadSettings.Instance.DefaultPath = DefaultPath;
            DownloadSettings.Instance.BufferBlockSize = BufferBlockSize;
            DownloadSettings.Instance.ChunkCount = ChunkCount;
            DownloadSettings.Instance.MaximumBytesPerSecond = MaximumBytesPerSecond;
            DownloadSettings.Instance.UnitForMaximumBytesPerSecond = UnitForMaximumBytesPerSecond;
            DownloadSettings.Instance.MaxTryAgainOnFailover = MaxTryAgainOnFailover;
            DownloadSettings.Instance.MaximumMemoryBufferBytes = MaximumMemoryBufferBytes;
            DownloadSettings.Instance.UnitForMaximumMemoryBufferBytes = UnitForMaximumMemoryBufferBytes;
            DownloadSettings.Instance.ParallelDownload = ParallelDownload;
            DownloadSettings.Instance.ParallelCount = ParallelCount;
            DownloadSettings.Instance.Timeout = Timeout;
            DownloadSettings.Instance.RangeDownload = RangeDownload;
            DownloadSettings.Instance.RangeLow = RangeLow;
            DownloadSettings.Instance.RangeHigh = RangeHigh;
            DownloadSettings.Instance.ClearPackageOnCompletionWithFailure = ClearPackageOnCompletionWithFailure;
            DownloadSettings.Instance.MinimumSizeOfChunking = MinimumSizeOfChunking;
            DownloadSettings.Instance.ReserveStorageSpaceBeforeStartingDownload = ReserveStorageSpaceBeforeStartingDownload;
            DownloadSettings.Instance.UserAgent = UserAgent;
            DownloadSettings.Instance.ProxyUri = ProxyUri;
        }
    }
}
