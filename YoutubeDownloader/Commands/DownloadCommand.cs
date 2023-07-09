using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private readonly DownloadViewModel _downloadViewModel;
        private readonly NavigationService _downloadHistoryNavigationService;

        public DownloadCommand(DownloaderStore downloaderStore, DownloadViewModel downloadViewModel, NavigationService downloadHistoryNavigationService) 
        {
            _downloaderStore = downloaderStore;

            _downloadViewModel = downloadViewModel;
            _downloadHistoryNavigationService = downloadHistoryNavigationService;
            _downloadViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }


        public override bool CanExecute(object? parameter)
        {
            return !string.IsNullOrEmpty(_downloadViewModel.VideoUrl) && base.CanExecute(parameter);
        }

        public override async Task ExecuteAsync(object parameter)
        {
            try
            {
                await _downloaderStore.AddVideoToHistory(new Video("Testtitle", _downloadViewModel.VideoUrl, "5:00", "Testchannel", "https://shop.avicii.com/cdn/shop/products/Avicii_SS_Front_grande_75f00e8c-2f44-401c-bff2-36ee0940fa43.png?v=1562179903"));
            }
            catch (Exception) 
            {
                MessageBox.Show("Failed to save Video.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            _downloadHistoryNavigationService.Navigate();
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
