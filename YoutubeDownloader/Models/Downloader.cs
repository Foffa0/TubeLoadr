using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.Models
{
    internal class Downloader
    {
        private readonly Queue _queue;

        public Downloader() 
        {
            _queue = new Queue(); 
        }

        public IEnumerable<Video> GetQueuedVideos()
        {
            return _queue.GetVideoQueue();
        }
    }
}
