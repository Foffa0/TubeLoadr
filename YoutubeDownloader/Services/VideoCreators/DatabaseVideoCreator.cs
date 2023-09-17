using System;
using System.Threading.Tasks;
using YoutubeDownloader.DbContexts;
using YoutubeDownloader.DTOs;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.Services.VideoCreators
{
    internal class DatabaseVideoCreator : IVideoCreator
    {
        private readonly DownloaderDbContextFactory _dbContextFactory;

        public DatabaseVideoCreator(DownloaderDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task CreateVideo(DownloadedVideo video)
        {
            using (DownloaderDbContext context = _dbContextFactory.CreateDbContext())
            {
                DownloadedVideoDTO downloadedVideoDTO = ToDownloadedVideoDTO(video);

                context.Videos.Add(downloadedVideoDTO);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteVideo(DownloadedVideo video)
        {
            using (DownloaderDbContext context = _dbContextFactory.CreateDbContext())
            {
                DownloadedVideoDTO downloadedVideoDTO = ToDownloadedVideoDTO(video);
                Guid guid = Guid.NewGuid();
                context.Videos.Remove(downloadedVideoDTO);
                await context.SaveChangesAsync();
            }
        }

        public async Task CreateQueuedVideo(Video video)
        {
            using (DownloaderDbContext context = _dbContextFactory.CreateDbContext())
            {
                VideoDTO videoDTO = ToVideoDTO(video);

                context.QueuedVideos.Add(videoDTO);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteQueuedVideo(Video video)
        {
            using (DownloaderDbContext context = _dbContextFactory.CreateDbContext())
            {
                VideoDTO videoDTO = ToVideoDTO(video);

                context.QueuedVideos.Remove(videoDTO);
                await context.SaveChangesAsync();
            }
        }

        private VideoDTO ToVideoDTO(Video video)
        {
            return new VideoDTO()
            {
                Id = video.Id,
                Title = video.Title,
                Url = video.Url,
                Duration = video.Duration,
                Channel = video.Channel,
                Thumbnail = video.Thumbnail,
                Filename = video.Filename,
                FilePath = video.FilePath,
                Format = video.Format,
                StartTime = video.StartTime,
                EndTime = video.EndTime,
                Resolution = video.Resolution,
            };
        }

        private DownloadedVideoDTO ToDownloadedVideoDTO(DownloadedVideo video)
        {
            return new DownloadedVideoDTO()
            {
                Id = video.Id,
                Title = video.Title,
                Url = video.Url,
                Duration = video.Duration,
                Channel = video.Channel,
                Thumbnail = video.Thumbnail,
                FilePath = video.FilePath,
                Filename = video.Filename,
                Format = video.Format,
            };
        }
    }
}
