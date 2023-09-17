using System;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.ViewModels
{
    public class VideoViewModel : ViewModelBase
    {
        private readonly Video _video;

        public Guid Id => _video.Id;
        public string Title => _video.Title;
        public int Duration => _video.Duration;
        public string Thumbnail => _video.Thumbnail;
        public string Format => _video.Format;
        public string DownloadState => _video.DownloadState;

        public VideoViewModel(Video video)
        {
            _video = video;
        }
    }
}
