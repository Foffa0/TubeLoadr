using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.Services.yt_dlp;

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

        public string Filename { get; }
        public string FilePath { get; }
        public string Format { get; }
        public int StartTime { get; }
        public int EndTime { get; }
        public string Resolution { get; }

        public string DownloadState { get; set; }

        public Video(string title, string url, int duration, string channel, string thumbnail, string filename, string filePath, string format, int startTime, int endTime, string resolution)
        {
            Id = Guid.NewGuid();
            Title = title;
            Url = url;
            Duration = duration;
            Channel = channel;
            Thumbnail = thumbnail;
            Filename = filename;
            FilePath = filePath;
            Format = format;
            StartTime = startTime;
            EndTime = endTime;
            Resolution = resolution;
            DownloadState = YtdlpDownloader.QUEUED;
        }

        public Video(Guid id, string title, string url, int duration, string channel, string thumbnail, string filename, string filePath, string format, int startTime, int endTime, string resolution)
        {   
            Id = id;
            Title = title;
            Url = url;
            Duration = duration;
            Channel = channel;
            Thumbnail = thumbnail;
            Filename = filename;
            FilePath = filePath;
            Format = format;
            StartTime = startTime;
            EndTime = endTime;
            Resolution = resolution;
            DownloadState = YtdlpDownloader.QUEUED;
        }
    }
}
