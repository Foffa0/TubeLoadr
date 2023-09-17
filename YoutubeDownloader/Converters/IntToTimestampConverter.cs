using System;
using System.Globalization;
using System.Windows.Data;

namespace YoutubeDownloader.Converters
{
    internal class IntToTimestampConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan result = TimeSpan.FromSeconds((int)value);
            string fromTimeString = result.ToString("hh':'mm':'ss");
            return fromTimeString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
