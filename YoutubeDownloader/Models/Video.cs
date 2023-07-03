using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.Models
{
    public class Video
    {
        public string Title { get; }
        public string Url { get; }
        public string Duration { get; }
        public string Channel { get; }
        public string Thumbnail { get; }

        public Video(string title, string url, string duration, string channel, string thumbnail)
        {
            Title = title;
            Url = url;
            Duration = duration;
            Channel = channel;
            Thumbnail = thumbnail;
        }
    }
}
