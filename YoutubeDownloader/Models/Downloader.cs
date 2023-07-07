using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.DbContexts;
using YoutubeDownloader.Services.VideoCreators;
using YoutubeDownloader.Services.VideoProviders;

namespace YoutubeDownloader.Models
{
    public class Downloader
    {

        private readonly IVideoCreator _videoCreator;
        private readonly IVideoProvider _videoProvider;

        private readonly Queue _queue;

        public Downloader(IVideoProvider videoProvider, IVideoCreator videoCreator) 
        {   
            _videoProvider = videoProvider;
            _videoCreator = videoCreator;

            _queue = new Queue(); 
        }

        /// <summary>
        /// Get all videos that are in the video queue.
        /// </summary>
        /// <returns>A list of Videos</returns>
        public IEnumerable<Video> GetQueuedVideos()
        {
            return _queue.GetVideoQueue();
        }

        public async Task<IEnumerable<Video>> GetVideoHistory()
        {
            return await _videoProvider.GetAllVideos();
        }

        public async Task AddVideoToHistory(Video video)
        {
            await _videoCreator.CreateVideo(video);
        }
    }
}
