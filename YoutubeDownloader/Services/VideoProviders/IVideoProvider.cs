using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.Services.VideoProviders
{
    public interface IVideoProvider
    {
        Task<IEnumerable<Video>> GetAllVideos();
    }
}
