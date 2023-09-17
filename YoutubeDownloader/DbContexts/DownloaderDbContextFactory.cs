using Microsoft.EntityFrameworkCore;

namespace YoutubeDownloader.DbContexts
{
    public class DownloaderDbContextFactory
    {
        private readonly string _connectionString;

        public DownloaderDbContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DownloaderDbContext CreateDbContext()
        {
            DbContextOptions options = new DbContextOptionsBuilder().UseSqlite(_connectionString).Options;

            return new DownloaderDbContext(options);
        }
    }
}
