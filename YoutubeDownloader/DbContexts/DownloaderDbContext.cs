using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.DTOs;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.DbContexts
{
    public class DownloaderDbContext : DbContext
    {
        public DownloaderDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<VideoDTO> Videos { get; set; }
    }
}
