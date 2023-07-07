using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.DbContexts;
using YoutubeDownloader.DTOs;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.Services.VideoProviders
{
    internal class DatabaseVideoProvider : IVideoProvider
    {
        private readonly DownloaderDbContextFactory _dbContextFactory;

        public DatabaseVideoProvider(DownloaderDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<IEnumerable<Video>> GetAllVideos()
        {
            using (DownloaderDbContext context = _dbContextFactory.CreateDbContext())
            {
                IEnumerable<VideoDTO> videoDTOs = await context.Videos.ToListAsync();

                return videoDTOs.Select(r => ToVideo(r));
            }
        }

        private static Video ToVideo(VideoDTO r)
        {
            return new Video(r.Title, r.Url, r.Duration, r.Channel, r.Thumbnail);
        }
    }
}
