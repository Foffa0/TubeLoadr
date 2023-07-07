using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.DbContexts;
using YoutubeDownloader.DTOs;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services.VideoProviders;

namespace YoutubeDownloader.Services.VideoCreators
{
    internal class DatabaseVideoCreator : IVideoCreator
    {
        private readonly DownloaderDbContextFactory _dbContextFactory;

        public DatabaseVideoCreator(DownloaderDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task CreateVideo(Video video)
        {
            using (DownloaderDbContext context = _dbContextFactory.CreateDbContext())
            {
                VideoDTO videoDTO = ToVideoDTO(video);

                context.Videos.Add(videoDTO);
                await context.SaveChangesAsync();
            }
        }

        private VideoDTO ToVideoDTO(Video video)
        {
            return new VideoDTO()
            {
                Title = video.Title,
                Url = video.Url,
                Duration = video.Duration,
                Channel = video.Channel,
                Thumbnail = video.Thumbnail,
            };
        }
    }
}
