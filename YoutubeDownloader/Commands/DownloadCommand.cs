using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;
using YoutubeDownloader.Stores;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader.Commands
{
    class DownloadCommand : AsyncCommandBase
    {
        private readonly DownloaderStore _downloaderStore;
        private readonly Downloader _downloader;
        private readonly DownloadViewModel _downloadViewModel;

        public DownloadCommand(DownloaderStore downloaderStore, Downloader downloader, DownloadViewModel downloadViewModel) 
        {
            _downloaderStore = downloaderStore;
            _downloader = downloader;

            _downloadViewModel = downloadViewModel;
            _downloadViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }


        public override bool CanExecute(object? parameter)
        {
            return !string.IsNullOrEmpty(_downloadViewModel.VideoUrl) && !_downloadViewModel.HasErrors && base.CanExecute(parameter);
        }

        public override async Task ExecuteAsync(object parameter)
        {
            /*try
            {*/
            string resolution = _downloadViewModel.IsVideoFormat ? _downloadViewModel.ResolutionVideo : _downloadViewModel.ResolutionAudio.ToString();


            DownloadOptions downloadOptions = new DownloadOptions(_downloadViewModel.OutputDir, _downloadViewModel.Format, _downloadViewModel.TimestampStart, _downloadViewModel.TimestampEnd, resolution);
            
            await _downloader.GetVideoInfoAndAddToQueue(_downloadViewModel.VideoUrl, downloadOptions);
            _downloadViewModel.VideoUrl = "";
            /*}
            catch (Exception) 
            {
                MessageBox.Show("Failed to save Video.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            */
        }


        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DownloadViewModel.VideoUrl))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
