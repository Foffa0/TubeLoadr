using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ICommand LoadHistoryVideosCommand { get; }
        public ICommand LoadDownloadViewCommand { get; }


        public DownloadHistoryViewModel(DownloaderStore downloaderStore, NavigationService historyViewNavigationService)
        {
            _downloaderStore = downloaderStore;

            LoadHistoryVideosCommand = new LoadHistoryVideosCommand(this, _downloaderStore);

            LoadDownloadViewCommand = new NavigateCommand(historyViewNavigationService);

            _videos = new ObservableCollection<DownloadedVideoViewModel>();

            _downloaderStore.VideoCreated += OnVideoCreated;
        }

        public override void Dispose()
        {   
            _downloaderStore.VideoCreated -= OnVideoCreated;
            base.Dispose();
        }

        private void OnVideoCreated(DownloadedVideo video)
        {
            DownloadedVideoViewModel videoViewModel = new DownloadedVideoViewModel(video);
            _videos.Add(videoViewModel);
        }


        public static DownloadHistoryViewModel LoadViewModel(DownloaderStore downloaderStore, NavigationService historyViewNavigationService)
        {
            DownloadHistoryViewModel viewModel = new DownloadHistoryViewModel(downloaderStore, historyViewNavigationService);
            viewModel.LoadHistoryVideosCommand.Execute(null);
            return viewModel;
        }


        public void UpdateVideos(IEnumerable<DownloadedVideo> videos)
        {
            _videos.Clear();

            foreach (DownloadedVideo video in videos)
            {
                DownloadedVideoViewModel v = new DownloadedVideoViewModel(video);
                _videos.Add(v);
            }
        }

    }
}

