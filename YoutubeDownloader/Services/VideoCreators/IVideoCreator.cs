using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.Services.VideoCreators
{
    public interface IVideoCreator
    {
        Task CreateVideo(DownloadedVideo video);

        Task CreateQueuedVideo(Video video);

        Task DeleteVideo(DownloadedVideo video);

        Task DeleteQueuedVideo(Video video);
    }
}
