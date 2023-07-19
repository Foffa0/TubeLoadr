using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.Models
{   
    /// <summary>
    /// Represents a Video that is in queue.
    /// </summary>
    public class Video
    {
        public Guid Id { get; }
        public string Title { get; }
        public string Url { get; }
        public int Duration { get; }
        public string Channel { get; }
        public string Thumbnail { get; }

        public string FilePath { get; }
        //TODO: Store all download options

        public Video(string title, string url, int duration, string channel, string thumbnail, string filePath)
        {
            Id = Guid.NewGuid();
            Title = title;
            Url = url;
            Duration = duration;
            Channel = channel;
            Thumbnail = thumbnail;
            FilePath = filePath;
        }

        public Video(Guid id, string title, string url, int duration, string channel, string thumbnail, string filePath)
        {   
            Id = id;
            Title = title;
            Url = url;
            Duration = duration;
            Channel = channel;
            Thumbnail = thumbnail;
            FilePath = filePath;
        }
    }
}
