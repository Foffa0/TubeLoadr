using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly Downloader _downloader;
        private string _videoUrl;
        private readonly ObservableCollection<VideoViewModel> _videos;

        public IEnumerable<VideoViewModel> Videos => _videos;

        public ICommand DownloadCommand { get; }
        public ICommand AboutCommand { get; }

        public DownloadViewModel(Downloader downloader, NavigationService aboutViewNavigationService)
        {
            DownloadCommand = new DownloadCommand(this);
            AboutCommand = new NavigateCommand(aboutViewNavigationService);
            
            _downloader = downloader;
            _videos = new ObservableCollection<VideoViewModel>();

            UpdateVideos();

            _videos.Add(new VideoViewModel(new Models.Video("Tenacious D - Peaches", "https://www.youtube.com/watch?v=2FPFgW0xVB0&list=PLA8ZIAm2I03hS41Fy4vFpRw8AdYNBXmNm&index=3", "5:35", "Tenacious D", "https://i.ytimg.com/vi/wxznTygnRfQ/maxresdefault.jpg")));
        }

        private void UpdateVideos()
        {
            _videos.Clear();

            foreach (Video video in _downloader.GetQueuedVideos())
            {
                VideoViewModel v = new VideoViewModel(video);
                _videos.Add(v);
            }
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

    }
}
