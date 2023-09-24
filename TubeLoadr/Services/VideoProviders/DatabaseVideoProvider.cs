using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TubeLoadr.DbContexts;
using TubeLoadr.DTOs;
using TubeLoadr.Models;

namespace TubeLoadr.Services.VideoProviders
{
    internal class DatabaseVideoProvider : IVideoProvider
    {
        private readonly DownloaderDbContextFactory _dbContextFactory;

        public DatabaseVideoProvider(DownloaderDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<IEnumerable<DownloadedVideo>> GetAllVideos()
        {
            using (DownloaderDbContext context = _dbContextFactory.CreateDbContext())
            {
                IEnumerable<DownloadedVideoDTO> videoDTOs = await context.Videos.ToListAsync();

                return videoDTOs.Select(r => ToDownloadedVideo(r));
            }
        }

        public async Task<IEnumerable<Video>> GetAllQueuedVideos()
        {
            using (DownloaderDbContext context = _dbContextFactory.CreateDbContext())
            {
                IEnumerable<VideoDTO> videoDTOs = await context.QueuedVideos.ToListAsync();

                return videoDTOs.Select(r => ToVideo(r));
            }
        }

        private static Video ToVideo(VideoDTO r)
        {
            return new Video(r.Id, r.Title, r.Url, r.Duration, r.Channel, r.Thumbnail, r.Filename, r.FilePath, r.Format, r.StartTime, r.EndTime, r.Resolution);
        }

        private static DownloadedVideo ToDownloadedVideo(DownloadedVideoDTO r)
        {
            return new DownloadedVideo(r.Id, r.Title, r.Url, r.Duration, r.Channel, r.Thumbnail, r.Format, r.FilePath, r.Filename);
        }
    }
}
