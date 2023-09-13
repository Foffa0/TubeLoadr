using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using YoutubeDownloader.Services.VideoCreators;
using YoutubeDownloader.Services.VideoProviders;
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


        public Downloader(DownloaderStore downloaderStore) 
        {
            _downloaderStore = downloaderStore;

            _ytdlpDownloader = new YtdlpDownloader();

            _downloaderState = YtdlpDownloader.DownloaderState.Ready;
            _downloaderStore.DownloaderState = _downloaderState;
        }

        public async Task GetVideoInfoAndAddToQueue(string url, DownloadOptions options)
        {
            Video video = await _ytdlpDownloader.AddToQueue(url, options);

            await _downloaderStore.AddVideoToQueue(video);
            DownloadVideo();
        }

        public async Task<VideoInfo> GetVideoInfo(string url)
        {
            VideoInfo video = await _ytdlpDownloader.GetVideoInfo(url);

            return video;
        }

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
