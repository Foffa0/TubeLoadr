using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YoutubeDownloader.Commands;

namespace YoutubeDownloader.ViewModels
{
    class DownloadViewModel : ViewModelBase
    {
        private string _videoUrl;
        private readonly ObservableCollection<VideoViewModel> _videos;

        public IEnumerable<VideoViewModel> Videos => _videos;

        public DownloadViewModel()
        {
            DownloadCommand = new DownloadCommand();

            _videos = new ObservableCollection<VideoViewModel>();

            _videos.Add(new VideoViewModel(new Models.Video("Tenacious D - Peaches", "https://www.youtube.com/watch?v=2FPFgW0xVB0&list=PLA8ZIAm2I03hS41Fy4vFpRw8AdYNBXmNm&index=3", "5:35", "Tenacious D", "https://i.ytimg.com/vi/wxznTygnRfQ/maxresdefault.jpg")));
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
        public ICommand DownloadCommand { get; }

       
        
    }
}
