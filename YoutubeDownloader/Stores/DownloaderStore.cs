using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.Stores
{
    public class DownloaderStore
    {
        private readonly Downloader _downloader;
        private readonly Lazy<Task> _initializeLazy;
        private readonly List<DownloadedVideo> _videos;
        private readonly List<Video> _downloadQueue;

        public Downloader Downloader { get { return _downloader; } }

        public IEnumerable<DownloadedVideo> Videos => _videos;

        public IEnumerable<Video> DownloadQueue => _downloadQueue;

        public event Action<DownloadedVideo> VideoCreated;
        public event Action<Video> QueuedVideoCreated;

        public DownloaderStore(Downloader downloader)
        {
            _downloader = downloader;
            _initializeLazy = new Lazy<Task>(Initialize);

            _videos = new List<DownloadedVideo>();
            _downloadQueue = new List<Video>();

            _downloader.VideoAddQueue += OnVideoAddedToQueue;
        }


        public async Task Load()
        {
            await _initializeLazy.Value;
        }
        private async void OnVideoAddedToQueue(Video video)
        {
            await AddVideoToQueue(video);
        }

        public async Task AddVideoToHistory(DownloadedVideo video)
        {
            await _downloader.AddVideoToHistory(video);

            _videos.Add(video);

            OnVideoCreated(video);
        }

        public async Task AddVideoToQueue(Video video)
        {
            await _downloader.AddVideoToQueue(video);

            _downloadQueue.Add(video);

            OnQueuedVideoCreated(video);
        }

        private void OnQueuedVideoCreated(Video video)
        {
            QueuedVideoCreated?.Invoke(video);
        }

        private void OnVideoCreated(DownloadedVideo video)
        {
            VideoCreated?.Invoke(video);
        }

        public async Task DeleteDownloadedVideo(DownloadedVideo video)
        {
            await _downloader.DeleteDownloadedVideo(video);
            _videos.Remove(video);
        }

        public async Task DeleteQueuedVideo(Video video)
        {
            await _downloader.DeleteQueuedVideo(video);
            _downloadQueue.Remove(video);
        }

        private async Task Initialize()
        {
            IEnumerable<DownloadedVideo> videos = await _downloader.GetVideoHistory();
            _videos.Clear();
            _videos.AddRange(videos);

            IEnumerable<Video> queuedVideos = await _downloader.GetQueuedVideos();
            _downloadQueue.Clear();
            _downloadQueue.AddRange(queuedVideos);

        }
    }
}
