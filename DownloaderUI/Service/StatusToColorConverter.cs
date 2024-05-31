using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace DownloaderUI.Service
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = (string)value;
            return status switch
            {
                "Completed" => Brushes.Green,
                "Error" => Brushes.Red,
                "Downloading" => Brushes.Black,
                "Pause" => Brushes.Tan,
                _ => Brushes.Black,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
