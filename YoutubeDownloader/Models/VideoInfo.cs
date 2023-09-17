using System.Collections.Generic;

namespace YoutubeDownloader.Models
{
    public class VideoInfo
    {
        public string Title { get; }
        public string Url { get; }
        public int Duration { get; }
        public string Channel { get; }
        public string Thumbnail { get; }

        public string FilePath { get; }
        public string Format { get; }
        public int StartTime { get; }
        public int EndTime { get; }
        public List<string> AvailableResolutionsVideo { get; }
        public List<int> AvailableResolutionsAudio { get; }

        public VideoInfo(string title, string url, int duration, string channel, string thumbnail, List<string> availableResolutionsVideo, List<int> availableResolutionsAudio)
        {
            Title = title;
            Url = url;
            Duration = duration;
            Channel = channel;
            Thumbnail = thumbnail;
            AvailableResolutionsVideo = availableResolutionsVideo;
            AvailableResolutionsAudio = availableResolutionsAudio;
        }
    }
}
