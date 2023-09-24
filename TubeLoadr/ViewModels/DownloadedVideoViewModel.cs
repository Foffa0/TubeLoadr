using System;
using TubeLoadr.Models;

namespace TubeLoadr.ViewModels
{
    public class DownloadedVideoViewModel : ViewModelBase
    {
        private readonly DownloadedVideo _video;

        public Guid Id => _video.Id;
        public string Title => _video.Title;
        public int Duration => _video.Duration;
        public string Thumbnail => _video.Thumbnail;
        public string Format => _video.Format;
        public string FilePath => _video.FilePath;
        public string Filename => _video.Filename;

        public DownloadedVideoViewModel(DownloadedVideo video)
        {
            _video = video;
        }
    }
}
