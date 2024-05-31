﻿using Avalonia;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using System;

namespace DownloaderUI
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI()
                .With(new FontManagerOptions
                {
                    DefaultFamilyName = "avares://DownloaderUI/Assets/Fonts#Noto",
                    FontFallbacks =
                    [
                        new FontFallback
                        {
                            FontFamily = new FontFamily("avares://DownloaderUI/Assets/Fonts#Noto")
                        }
                    ]
                });
    }
}
