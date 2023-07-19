using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YoutubeDownloader.Commands;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;
using YoutubeDownloader.Stores;

namespace YoutubeDownloader.ViewModels
{
    class DownloadViewModel : ViewModelBase
    {
        private readonly DownloaderStore _downloaderStore;
        private string _videoUrl;
        private readonly ObservableCollection<VideoViewModel> _videos;

        public IEnumerable<VideoViewModel> Videos => _videos;

        public ICommand DownloadCommand { get; }
        public ICommand DownloadHistoryCommand { get; }
        public ICommand AboutCommand { get; }
        public ICommand LoadQueuedVideosCommand { get; }

        public DownloadViewModel(DownloaderStore downloaderStore, NavigationService aboutViewNavigationService, NavigationService downloadHistoryNavigationService)
        {
            _downloaderStore = downloaderStore;

            DownloadCommand = new DownloadCommand(_downloaderStore, _downloaderStore.Downloader, this);
            DownloadHistoryCommand = new NavigateCommand(downloadHistoryNavigationService);
            AboutCommand = new NavigateCommand(aboutViewNavigationService);

            LoadQueuedVideosCommand = new LoadQueuedVideosCommand(this, downloaderStore);

            _videos = new ObservableCollection<VideoViewModel>();

            _downloaderStore.QueuedVideoCreated += OnVideoCreated;
            _downloaderStore.QueuedVideoDeleted += OnVideoDeleted;
        }


        public override void Dispose()
        {
            _downloaderStore.QueuedVideoCreated -= OnVideoCreated;
            _downloaderStore.QueuedVideoDeleted -= OnVideoDeleted;
            base.Dispose();
        }

        public static DownloadViewModel LoadViewModel(DownloaderStore downloaderStore, NavigationService downloadViewNavigationService, NavigationService aboutViewNavigationService)
        {
            DownloadViewModel viewModel = new DownloadViewModel(downloaderStore, aboutViewNavigationService, downloadViewNavigationService);
            viewModel.LoadQueuedVideosCommand.Execute(null);
            return viewModel;
        }


        private void OnVideoCreated(Video video)
        {
            VideoViewModel videoViewModel = new VideoViewModel(video);
            _videos.Add(videoViewModel);
        }
        
        private void OnVideoDeleted(Video video)
        {
            VideoViewModel videoViewModel = new VideoViewModel(video);
            _videos.Remove(_videos.Where(i => i.Id == video.Id).Single());
        }

        public string VideoUrl
        {
            get
            {
                return _videoUrl;
            }
            set
            {
                _videoUrl = value;
                OnPropertyChanged(nameof(VideoUrl));
            }
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
