using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TubeLoadr.Services.yt_dlp;

namespace TubeLoadr.Converters
{
    internal class InverseDownloadStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Equals(YtdlpDownloader.DOWNLOADING)) return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
