using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services.VideoCreators;
using YoutubeDownloader.Services.VideoProviders;
using YoutubeDownloader.Services.yt_dlp;

namespace YoutubeDownloader.Stores
{
    public class DownloaderStore
    {
        private Lazy<Task> _initializeLazy;
        private IVideoProvider _videoProvider;
        private IVideoCreator _videoCreator;

        private readonly List<DownloadedVideo> _videos;
        private readonly List<Video> _downloadQueue;

        private YtdlpDownloader.DownloaderState _downloaderState;
        public YtdlpDownloader.DownloaderState DownloaderState 
        { 
            get { return _downloaderState; } 
            set 
            { 
                _downloaderState = value;
                DownloaderStateChanged?.Invoke(this, EventArgs.Empty);
            } 
        }
        public event EventHandler DownloaderStateChanged;

        public IEnumerable<DownloadedVideo> Videos => _videos;

        public IEnumerable<Video> DownloadQueue => _downloadQueue;

        public event Action<DownloadedVideo> DownloadedVideoCreated;
        public event Action<DownloadedVideo> DownloadedVideoDeleted;
        public event Action<Video> QueuedVideoCreated;
        public event Action<Video> QueuedVideoDeleted;
        public event EventHandler QueuedVideoUpdated;        


        public DownloaderStore(IVideoProvider videoProvider, IVideoCreator videoCreator)
        {
            _videoProvider = videoProvider;
            _videoCreator = videoCreator;
            _initializeLazy = new Lazy<Task>(Initialize);

            _videos = new List<DownloadedVideo>();
            _downloadQueue = new List<Video>();
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


        /*private void OnVideoAddedToQueue(Video video)
        {
            _downloadQueue.Add(video);
            QueuedVideoCreated?.Invoke(video);

            /*_downloader.DownloadQueue = _downloadQueue;
            _downloader.DownloadVideo();*/
        //}

        /*private void OnVideoRemovedFromQueue(Video video)
        {
            _downloadQueue.Remove(_downloadQueue.Where(i => i.Id == video.Id).Single());
            //_downloader.DownloadQueue = _downloadQueue;
            QueuedVideoDeleted?.Invoke(video);
        }*/
        /*
        private void OnDownloadedVideoCreated(DownloadedVideo video)
        {   
            _videos.Add(video);
            DownloadedVideoCreated?.Invoke(video);
        }*/

        /*private void OnDownloadedVideoRemoved(DownloadedVideo video)
        {
            _videos.Remove(_videos.Where(i => i.Id == video.Id).Single());
            DownloadedVideoDeleted?.Invoke(video);
        }*/


        /*public async Task AddVideoToHistory(DownloadedVideo video)
        {
            await _downloader.AddVideoToHistory(video);

            _videos.Add(video);

            OnDownloadedVideoCreated(video);
        }*/



       /* public async Task DeleteDownloadedVideo(DownloadedVideo video)
        {
             await _videoCreator.DeleteVideo(video);
            OnDownloadedVideoRemoved(video);
        }*/

        /* public async Task DeleteQueuedVideo(Video video)
         {
             await _downloader.DeleteQueuedVideo(video);
             _downloadQueue.Remove(video);
         }*/

        public async Task AddVideoToHistory(DownloadedVideo video)
        {
            await _videoCreator.CreateVideo(video);
            _videos.Add(video);
            DownloadedVideoCreated?.Invoke(video);
        }

        public async Task DeleteDownloadedVideo(DownloadedVideo downloadedVideo)
        {
            await _videoCreator.DeleteVideo(downloadedVideo);
            _videos.Remove(_videos.Where(i => i.Id == downloadedVideo.Id).Single());
            DownloadedVideoDeleted?.Invoke(downloadedVideo);
        }
        public async Task AddVideoToQueue(Video video)
        {
            await _videoCreator.CreateQueuedVideo(video);
            _downloadQueue.Add(video);
            QueuedVideoCreated?.Invoke(video);
        }

        public async Task DeleteQueuedVideo(Video video)
        {
            await _videoCreator.DeleteQueuedVideo(video);
            _downloadQueue.Remove(_downloadQueue.Where(i => i.Id == video.Id).Single());
            QueuedVideoDeleted?.Invoke(video);
        }

        public void QueuedVideoUpdate()
        {
            QueuedVideoUpdated?.Invoke(this, EventArgs.Empty);
        }

        private async Task Initialize()
        {
            IEnumerable<DownloadedVideo> videos = await _videoProvider.GetAllVideos();
            _videos.Clear();
            _videos.AddRange(videos);

            IEnumerable<Video> queuedVideos = await _videoProvider.GetAllQueuedVideos();
            _downloadQueue.Clear();
            _downloadQueue.AddRange(queuedVideos);
        }
    }
}
