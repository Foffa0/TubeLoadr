using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.ViewModels
{
    public class DownloadedVideoViewModel : ViewModelBase
    {
        private readonly DownloadedVideo _video;

        public string Title => _video.Title;
        public int Duration => _video.Duration;
        public string Thumbnail => _video.Thumbnail;
        public string FilePath => _video.FilePath;

        public DownloadedVideoViewModel(DownloadedVideo video)
        {
            _video = video;
        }
    }
}
