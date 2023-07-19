﻿using Microsoft.EntityFrameworkCore;
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


        public Downloader(IVideoProvider videoProvider, IVideoCreator videoCreator) 
        {   
            _videoProvider = videoProvider;
            _videoCreator = videoCreator;

            _ytdlpDownloader = new YtdlpDownloader();
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

        public event Action<Video> VideoCreated;
        public event Action<Video> VideoDeleted;
        public event Action<DownloadedVideo> DownloadedVideoCreated;
        public event Action<DownloadedVideo> DownloadedVideoDeleted;

        public async Task AddToQueue(string url)
        {
            Video video = await _ytdlpDownloader.AddToQueue(url);

            await AddVideoToQueue(video);
            VideoCreated?.Invoke(video);
        }

        public async void DownloadVideo()
        {
            IEnumerable<Video> v = await GetQueuedVideos();
            List<Video> videos = v.ToList();

            await _ytdlpDownloader.DownloadVideo(videos.First());

            await DeleteQueuedVideo(videos.First());
            VideoDeleted?.Invoke(videos.First());

            DownloadedVideo vid = new DownloadedVideo(videos.First().Title, videos.First().Url, videos.First().Duration, videos.First().Channel, videos.First().Thumbnail, videos.First().FilePath);
            DownloadedVideoCreated?.Invoke(vid);

            videos.Remove(videos.First());
            if (videos.Count() > 0)
            {
                DownloadVideo();
            }
        }

        public async Task AddVideoToQueue(Video video)
        {
            await _videoCreator.CreateQueuedVideo(video);
        }

        public async Task DeleteQueuedVideo(Video video)
        {
            await _videoCreator.DeleteQueuedVideo(video);
        }
    }
}
