using Avalonia.Data.Converters;
using FluentAvalonia.UI.Controls;
using System;
using System.Globalization;
using System.Reactive;

namespace DownloaderUI.Service
{
    public class StatusToSvgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = (string)value;
            return status switch
            {
                "Completed" => Symbol.Checkmark,
                "Error" => Symbol.Cancel,
                "Downloading" => Symbol.Clock,
                "Pause" => Symbol.Pause,
                _ => Unit.Default,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
