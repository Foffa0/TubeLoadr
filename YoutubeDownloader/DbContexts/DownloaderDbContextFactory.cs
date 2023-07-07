using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
