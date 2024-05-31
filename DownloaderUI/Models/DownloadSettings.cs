using System;

namespace DownloaderUI.Models
{
    public class DownloadSettings
    {
        // Lazy<T> is a class in .NET used for lazy initialization of objects
        // It ensures that the instance is created only when it is first used and is thread-safe
        private static readonly Lazy<DownloadSettings> _instance = new(() => new DownloadSettings());

        // Public static property to get the single instance
        public static DownloadSettings Instance => _instance.Value;

        // Private constructor to prevent instance creation from outside
        private DownloadSettings() { }

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
        public string? DefaultPath { get; set; }
    }
}
