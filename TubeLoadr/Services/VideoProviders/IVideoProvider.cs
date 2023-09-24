using System.Collections.Generic;
using System.Threading.Tasks;
using TubeLoadr.Models;

namespace TubeLoadr.Services.VideoProviders
{
    public interface IVideoProvider
    {
        Task<IEnumerable<DownloadedVideo>> GetAllVideos();

        Task<IEnumerable<Video>> GetAllQueuedVideos();
    }
}
