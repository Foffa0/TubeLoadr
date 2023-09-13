﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.Models
{
    public class DownloadedVideo
    {
        public Guid Id { get; }
        public string Title { get; }
        public string Url { get; }
        public int Duration { get; }
        public string Channel { get; }
        public string Thumbnail { get; }
        public string Format { get; }

        public string FilePath { get; }
        public string Filename { get; }

        public DownloadedVideo(string title, string url, int duration, string channel, string thumbnail, string format, string filePath, string filename)
        {
            Id = Guid.NewGuid();
            Title = title;
            Url = url;
            Duration = duration;
            Channel = channel;
            Thumbnail = thumbnail;
            FilePath = filePath;
            Filename = filename;
            Format = format;
        }

        public DownloadedVideo(Guid id, string title, string url, int duration, string channel, string thumbnail, string format, string filePath, string filename)
        {
            Id = id;
            Title = title;
            Url = url;
            Duration = duration;
            Channel = channel;
            Thumbnail = thumbnail;
            FilePath = filePath;
            Filename = filename;
            Format = format;
        }
    }
}
