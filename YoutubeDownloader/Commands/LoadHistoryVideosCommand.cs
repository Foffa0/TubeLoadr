﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YoutubeDownloader.Models;
using YoutubeDownloader.Stores;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader.Commands
{
    public class LoadHistoryVideosCommand : AsyncCommandBase
    {
        private readonly DownloadHistoryViewModel _viewModel;
        private readonly DownloaderStore _downloaderStore;

        public LoadHistoryVideosCommand(DownloadHistoryViewModel viewModel, DownloaderStore downloaderStore)
        {
            _viewModel = viewModel;
            _downloaderStore = downloaderStore;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            _viewModel.IsLoading = true;
            try
            {
                await _downloaderStore.Load();
                _viewModel.UpdateVideos(_downloaderStore.Videos);
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to load videos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine(e);
            }
            _viewModel.IsLoading = false;
        }
    }
}
