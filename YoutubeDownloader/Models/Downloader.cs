using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using YoutubeDownloader.Services.yt_dlp;
using YoutubeDownloader.Stores;

namespace YoutubeDownloader.Models
{
    public class Downloader
    {

        private readonly DownloaderStore _downloaderStore;

        private readonly YtdlpDownloader _ytdlpDownloader;
        private YtdlpDownloader.DownloaderState _downloaderState;
        public YtdlpDownloader.DownloaderState DownloaderState { get { return _downloaderState; } }


        public Downloader(DownloaderStore downloaderStore, ILogger<YtdlpDownloader> logger)
        {
            _downloaderStore = downloaderStore;

            _ytdlpDownloader = new YtdlpDownloader(logger);

            _downloaderState = YtdlpDownloader.DownloaderState.Ready;
            _downloaderStore.DownloaderState = _downloaderState;
        }

        /// <summary>
        /// Gets the metadata from a youtube url and adds the video to the downloadqueue.
        /// </summary>
        /// <param name="url">The youtube url.</param>
        /// <param name="options">Download options including the format and resolution.</param>
        /// <returns></returns>
        public async Task GetVideoInfoAndAddToQueue(string url, DownloadOptions options)
        {
            Video video = await _ytdlpDownloader.AddToQueue(url, options);

            await _downloaderStore.AddVideoToQueue(video);
            DownloadVideo();
        }

        /// <summary>
        /// Gets the metadata from a youtube url.
        /// </summary>
        /// <param name="url">The youtube url.</param>
        /// <returns>A <see cref="VideoInfo"/> object containing metadata information.</returns>
        public async Task<VideoInfo> GetVideoInfo(string url)
        {
            VideoInfo video = await _ytdlpDownloader.GetVideoInfo(url);

            return video;
        }


        /// <summary>
        /// Download the first video in queue.
        /// </summary>
        public async void DownloadVideo()
        {
            if (_downloaderState == YtdlpDownloader.DownloaderState.Downloading) { return; }
            if (_downloaderStore.DownloadQueue.Count() <= 0) { return; }

            _downloaderState = YtdlpDownloader.DownloaderState.Downloading;
            _downloaderStore.DownloaderState = _downloaderState;
            _downloaderStore.DownloadQueue.First().DownloadState = YtdlpDownloader.DOWNLOADING;
            _downloaderStore.QueuedVideoUpdate();
            try
            {
                await _ytdlpDownloader.DownloadVideo(_downloaderStore.DownloadQueue.First());
            }
            catch (TaskCanceledException e)
            {
                return;
            }
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show(e.Message,
                                          "Error while downloading",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Warning);
                await _downloaderStore.DeleteQueuedVideo(_downloaderStore.DownloadQueue.First());
                _downloaderState = YtdlpDownloader.DownloaderState.Ready;
                _downloaderStore.DownloaderState = _downloaderState;
                return;
            }

            DownloadedVideo vid = new DownloadedVideo(_downloaderStore.DownloadQueue.First().Title, _downloaderStore.DownloadQueue.First().Url, _downloaderStore.DownloadQueue.First().Duration, _downloaderStore.DownloadQueue.First().Channel, _downloaderStore.DownloadQueue.First().Thumbnail, _downloaderStore.DownloadQueue.First().Format, _downloaderStore.DownloadQueue.First().FilePath, _downloaderStore.DownloadQueue.First().Filename);

            await _downloaderStore.DeleteQueuedVideo(_downloaderStore.DownloadQueue.First());

            await _downloaderStore.AddVideoToHistory(vid);

            _downloaderState = YtdlpDownloader.DownloaderState.Ready;
            _downloaderStore.DownloaderState = _downloaderState;

            if (_downloaderStore.DownloadQueue.Count() > 0)
            {
                DownloadVideo();
            }
        }

        /// <summary>
        /// Cancels the current download.
        /// </summary>
        public void CancelDownload()
        {
            if (_downloaderState != YtdlpDownloader.DownloaderState.Downloading) { return; }
            _ytdlpDownloader.CancelDownload();
            _downloaderState = YtdlpDownloader.DownloaderState.Paused;
            _downloaderStore.DownloaderState = _downloaderState;
            _downloaderStore.DownloadQueue.First().DownloadState = YtdlpDownloader.QUEUED;
        }
    }
}
