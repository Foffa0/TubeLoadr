using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.Stores
{
    public class DownloaderStore
    {
        private readonly Downloader _downloader;
        private readonly Lazy<Task> _initializeLazy;
        private readonly List<Video> _videos;

        public IEnumerable<Video> Videos => _videos;

        public event Action<Video> VideoCreated;

        public DownloaderStore(Downloader downloader)
        {
            _downloader = downloader;
            _initializeLazy = new Lazy<Task>(Initialize);

            _videos = new List<Video>();
        }

        public async Task Load()
        {
            await _initializeLazy.Value;
        }

        public async Task AddVideoToHistory(Video video)
        {
            await _downloader.AddVideoToHistory(video);

            _videos.Add(video);

            OnVideoCretead(video);
        }

        private void OnVideoCretead(Video video)
        {
            VideoCreated?.Invoke(video);
        }

        private async Task Initialize()
        {
            IEnumerable<Video> videos = await _downloader.GetVideoHistory();

            _videos.Clear();
            _videos.AddRange(videos);
        }
    }
}
