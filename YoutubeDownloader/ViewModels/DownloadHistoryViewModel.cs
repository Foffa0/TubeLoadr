using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using YoutubeDownloader.Commands;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;
using YoutubeDownloader.Stores;

namespace YoutubeDownloader.ViewModels
{
    public class DownloadHistoryViewModel : ViewModelBase
    {
        private readonly DownloaderStore _downloaderStore;
        private readonly ObservableCollection<DownloadedVideoViewModel> _videos;

        public IEnumerable<DownloadedVideoViewModel> VideosHistory => _videos;

        private string _errorMessage;
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
                OnPropertyChanged(nameof(HasErrorMessage));
            }

        }

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);


        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set 
            { 
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }

        }

        public ICommand LoadHistoryVideosCommand { get; }
        public ICommand LoadDownloadViewCommand { get; }
        public ICommand RemoveFromHistoryCommand { get; }
        public ICommand OpenFolderCommand { get; }


        public DownloadHistoryViewModel(DownloaderStore downloaderStore, NavigationService<DownloadViewModel> downloadViewNavigationService)
        {
            _downloaderStore = downloaderStore;

            LoadHistoryVideosCommand = new LoadHistoryVideosCommand(this, _downloaderStore);

            LoadDownloadViewCommand = new NavigateCommand<DownloadViewModel>(downloadViewNavigationService);

            RemoveFromHistoryCommand = new RemoveFromHistoryCommand(this, _downloaderStore);

            OpenFolderCommand = new OpenFolderCommand();

            _videos = new ObservableCollection<DownloadedVideoViewModel>();

            _downloaderStore.DownloadedVideoCreated += OnVideoCreated;
            _downloaderStore.DownloadedVideoDeleted += OnVideoDeleted;
        }


        public override void Dispose()
        {   
            _downloaderStore.DownloadedVideoCreated -= OnVideoCreated;
            base.Dispose();
        }

        private void OnVideoCreated(DownloadedVideo video)
        {
            DownloadedVideoViewModel videoViewModel = new DownloadedVideoViewModel(video);
            _videos.Insert(0, videoViewModel);
        }

        private void OnVideoDeleted(DownloadedVideo video)
        {
            DownloadedVideoViewModel videoViewModel = new DownloadedVideoViewModel(video);
            _videos.Remove(_videos.Where(i => i.Id == video.Id).Single());
        }

        public static DownloadHistoryViewModel LoadViewModel(DownloaderStore downloaderStore, NavigationService<DownloadViewModel> downloadViewNavigationService)
        {
            DownloadHistoryViewModel viewModel = new DownloadHistoryViewModel(downloaderStore, downloadViewNavigationService);
            viewModel.LoadHistoryVideosCommand.Execute(null);
            return viewModel;
        }


        public void UpdateVideos(IEnumerable<DownloadedVideo> videos)
        {
            _videos.Clear();

            foreach (DownloadedVideo video in videos)
            {
                DownloadedVideoViewModel v = new DownloadedVideoViewModel(video);
                _videos.Insert(0, v);
            }
        }

    }
}

