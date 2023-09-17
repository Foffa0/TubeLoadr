﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using YoutubeDownloader.Services.yt_dlp;

namespace YoutubeDownloader.Converters
{
    internal class DownloadStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Equals(YtdlpDownloader.DOWNLOADING)) return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
