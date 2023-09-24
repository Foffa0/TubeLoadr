using Microsoft.EntityFrameworkCore;
using TubeLoadr.DTOs;

namespace TubeLoadr.DbContexts
{
    public class DownloaderDbContext : DbContext
    {
        public DownloaderDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<DownloadedVideoDTO> Videos { get; set; }

        public DbSet<VideoDTO> QueuedVideos { get; set; }
    }
}
