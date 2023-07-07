using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.DbContexts
{
    public class DownloaderDesignTimeDbContextFactory :  IDesignTimeDbContextFactory<DownloaderDbContext>
    {
        public DownloaderDbContext CreateDbContext(string[] args)
        {
            DbContextOptions options = new DbContextOptionsBuilder().UseSqlite("Data Source=downloader.db").Options;

            return new DownloaderDbContext(options);
        }
    }
}
