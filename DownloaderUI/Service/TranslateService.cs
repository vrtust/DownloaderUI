using Avalonia.Markup.Xaml.Styling;
using System;
using System.Linq;

namespace DownloaderUI.Service
{
    public class TranslateService
    {
        public static void Translate(string targetLanguage)
        {
            var translations = Avalonia.Application.Current.Resources.MergedDictionaries.OfType<ResourceInclude>().FirstOrDefault(x => x.Source?.OriginalString?.Contains("/Lang/") ?? false);

            if (translations != null)
                Avalonia.Application.Current.Resources.MergedDictionaries.Remove(translations);

            // var resource = AssetLoader.Open(new Uri($"avares://LocalizationSample/Assets/Lang/{targetLanguage}.axaml"));

            Avalonia.Application.Current.Resources.MergedDictionaries.Add(
                new ResourceInclude(new Uri($"avares://DownloaderUI/Assets/Lang/{targetLanguage}.axaml"))
                {
                    Source = new Uri($"avares://DownloaderUI/Assets/Lang/{targetLanguage}.axaml")
                });
        }
    }
}
