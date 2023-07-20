using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using YoutubeDownloader.DbContexts;
using YoutubeDownloader.DTOs;
using YoutubeDownloader.Migrations;
using YoutubeDownloader.Services.VideoCreators;
using YoutubeDownloader.Services.VideoProviders;
using YoutubeDownloader.Services.yt_dlp;
using YoutubeDownloader.Stores;

namespace YoutubeDownloader.Models
{
    public class Downloader
    {

        private readonly IVideoCreator _videoCreator;
        private readonly IVideoProvider _videoProvider;

        private readonly YtdlpDownloader _ytdlpDownloader;
        private bool _isDownloading;

        private List<Video> _downloadQueue;
        public List<Video> DownloadQueue { get { return _downloadQueue; } set { _downloadQueue = value; } }

        public Downloader(IVideoProvider videoProvider, IVideoCreator videoCreator) 
        {   
            _videoProvider = videoProvider;
            _videoCreator = videoCreator;

            _ytdlpDownloader = new YtdlpDownloader();

            _isDownloading = false;

            _downloadQueue = new List<Video>();
        }

        /// <summary>
        /// Get all videos that are in the video queue.
        /// </summary>
        /// <returns>A list of Videos</returns>
        public async Task<IEnumerable<Video>> GetQueuedVideos()
        {
            return await _videoProvider.GetAllQueuedVideos();
        }

        public async Task<IEnumerable<DownloadedVideo>> GetVideoHistory()
        {
            return await _videoProvider.GetAllVideos();
        }

        public async Task AddVideoToHistory(DownloadedVideo video)
        {
            await _videoCreator.CreateVideo(video);
        }

        public async Task DeleteDownloadedVideo(DownloadedVideo downloadedVideo)
        {
            await _videoCreator.DeleteVideo(downloadedVideo);
        }
        public async Task AddVideoToQueue(Video video)
        {
            await _videoCreator.CreateQueuedVideo(video);
        }

        public async Task DeleteQueuedVideo(Video video)
        {
            await _videoCreator.DeleteQueuedVideo(video);
        }


        public event Action<Video> VideoCreated;
        public event Action<Video> VideoDeleted;
        public event Action<DownloadedVideo> DownloadedVideoCreated;
        public event Action<DownloadedVideo> DownloadedVideoDeleted;


        public async Task GetVideoInfo(string url)
        {
            Video video = await _ytdlpDownloader.AddToQueue(url);

            await AddVideoToQueue(video);
            VideoCreated?.Invoke(video);
        }

        public async void DownloadVideo()
        {   
            if (_isDownloading) { return; }

            _isDownloading = true;

            await _ytdlpDownloader.DownloadVideo(_downloadQueue.First());

            DownloadedVideo vid = new DownloadedVideo(_downloadQueue.First().Title, _downloadQueue.First().Url, _downloadQueue.First().Duration, _downloadQueue.First().Channel, _downloadQueue.First().Thumbnail, _downloadQueue.First().FilePath);
            
            await DeleteQueuedVideo(_downloadQueue.First());
            VideoDeleted?.Invoke(_downloadQueue.First());

            await AddVideoToHistory(vid);
            DownloadedVideoCreated?.Invoke(vid);

            if (_downloadQueue.Count() > 0)
            {
                _isDownloading = false;
                DownloadVideo();
            }
            _isDownloading = false;
        }
    }
}
