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
        private Lazy<Task> _initializeLazy;
        private readonly List<DownloadedVideo> _videos;
        private readonly List<Video> _downloadQueue;

        public Downloader Downloader { get { return _downloader; } }

        public IEnumerable<DownloadedVideo> Videos => _videos;

        public IEnumerable<Video> DownloadQueue => _downloadQueue;

        public event Action<DownloadedVideo> DownloadedVideoCreated;
        public event Action<DownloadedVideo> DownloadedVideoDeleted;
        public event Action<Video> QueuedVideoCreated;
        public event Action<Video> QueuedVideoDeleted;

        public DownloaderStore(Downloader downloader)
        {
            _downloader = downloader;
            _initializeLazy = new Lazy<Task>(Initialize);

            _videos = new List<DownloadedVideo>();
            _downloadQueue = new List<Video>();

            _downloader.VideoCreated += OnVideoAddedToQueue;
            _downloader.VideoDeleted += OnVideoRemovedFromQueue;
            _downloader.DownloadedVideoCreated += OnDownloadedVideoCreated; //Todo
            _downloader.DownloadedVideoDeleted += OnDownloadedVideoRemoved;
        }

        public async Task Load()
        {
            try
            {
                await _initializeLazy.Value;
            }
            catch (Exception)
            {
                _initializeLazy = new Lazy<Task>(Initialize);
                throw;
            }
        }


        private void OnVideoAddedToQueue(Video video)
        {
            _downloadQueue.Add(video);
            QueuedVideoCreated?.Invoke(video);

            _downloader.DownloadQueue = _downloadQueue;
            _downloader.DownloadVideo();
        }

        private void OnVideoRemovedFromQueue(Video video)
        {
            _downloadQueue.Remove(_downloadQueue.Where(i => i.Id == video.Id).Single());
            _downloader.DownloadQueue = _downloadQueue;
            QueuedVideoDeleted?.Invoke(video);
        }

        private void OnDownloadedVideoCreated(DownloadedVideo video)
        {   
            _videos.Add(video);
            DownloadedVideoCreated?.Invoke(video);
        }

        private void OnDownloadedVideoRemoved(DownloadedVideo video)
        {
            _videos.Remove(_videos.Where(i => i.Id == video.Id).Single());
            DownloadedVideoDeleted?.Invoke(video);
        }


        /*public async Task AddVideoToHistory(DownloadedVideo video)
        {
            await _downloader.AddVideoToHistory(video);

            _videos.Add(video);

            OnDownloadedVideoCreated(video);
        }*/



        /* public async Task DeleteDownloadedVideo(DownloadedVideo video)
         {
             await _downloader.DeleteDownloadedVideo(video);
             _videos.Remove(video);
         }*/

        /* public async Task DeleteQueuedVideo(Video video)
         {
             await _downloader.DeleteQueuedVideo(video);
             _downloadQueue.Remove(video);
         }*/

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
