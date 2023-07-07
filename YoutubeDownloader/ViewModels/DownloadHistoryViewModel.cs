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
        private readonly ObservableCollection<VideoViewModel> _videos;

        public IEnumerable<VideoViewModel> VideosHistory => _videos;

        public ICommand LoadHistoryVideosCommand { get; }
        public ICommand LoadDownloadViewCommand { get; }

        public DownloadHistoryViewModel(DownloaderStore downloaderStore, NavigationService historyViewNavigationService)
        {
            _downloaderStore = downloaderStore;

            LoadHistoryVideosCommand = new LoadHistoryVideosCommand(this, _downloaderStore);

            LoadDownloadViewCommand = new NavigateCommand(historyViewNavigationService);

            _videos = new ObservableCollection<VideoViewModel>();
        }

        public static DownloadHistoryViewModel LoadViewModel(DownloaderStore downloaderStore, NavigationService historyViewNavigationService)
        {
            DownloadHistoryViewModel viewModel = new DownloadHistoryViewModel(downloaderStore, historyViewNavigationService);
            viewModel.LoadHistoryVideosCommand.Execute(null);
            return viewModel;
        }

        public void UpdateVideos(IEnumerable<Video> videos)
        {
            _videos.Clear();

            foreach (Video video in videos)
            {
                VideoViewModel v = new VideoViewModel(video);
                _videos.Add(v);
            }
        }

    }
}

