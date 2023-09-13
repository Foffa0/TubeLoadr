using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;
using YoutubeDownloader.Exceptions;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.Services.yt_dlp
{
    public class YtdlpDownloader
    {

        public static readonly string QUEUED = "Queued";
        public static readonly string DOWNLOADING = "Downloading";
        public static readonly string DOWNLOADED = "Finished";
        public static readonly string ERROR = "Error";

        public enum DownloaderState
        {
            Paused,
            Downloading,
            Ready
        }

        YoutubeDL ytdl;

        string? dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        public YtdlpDownloader()
        {
            ytdl = new YoutubeDL();
            ytdl.YoutubeDLPath = dir + @"\Downloadtools\yt-dlp.exe";
            ytdl.FFmpegPath = dir + @"\Downloadtools\ffmpeg.exe";
            Debug.WriteLine(ytdl.Version);
        }

        private CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// Get video metadata from url.
        /// </summary>
        /// <param name="url">The youtube video url.</param>
        /// <exception cref="VideoNotFoundException">Thrown if no video exists at the url.</exception>
        public async Task<VideoInfo> GetVideoInfo(string url)
        {
            var res = await ytdl.RunVideoDataFetch(url);

            if (res.ErrorOutput != null || res.ErrorOutput.Any())
            {
                Debug.WriteLine("------------------------------------------------------------------");
                Debug.WriteLine($"Error output: {res.ErrorOutput[0]}");
            }

            // get some video information
            VideoData video = res.Data;

            if (video == null) throw new VideoNotFoundException("Youtube video not found");

            string title = video.Title;
            string uploader = video.Uploader;
            long? views = video.ViewCount;
            // all available download formats
            FormatData[] formats = video.Formats;

            List<string> videoResolutions = new List<string>();
            List<int> audioResolutions = new List<int>();

            for (int i = 0; i < formats.Length; i++)
            {
                if (formats[i].VideoCodec != null && formats[i].VideoCodec != "none")
                {
                    if (formats[i].Height > 300 && videoResolutions.Where(x => x.Contains(formats[i].Height.ToString())).FirstOrDefault() == null)
                    {
                        videoResolutions.Add(formats[i].Height.ToString() + "p");
                    }
                }
                else if (formats[i].AudioCodec != null && formats[i].AudioBitrate != null)
                {
                    if (formats[i].AudioBitrate > 30.0 && !audioResolutions.Contains((int)Math.Round((double)formats[i].AudioBitrate)))
                    {
                        audioResolutions.Add((int)Math.Round((double)formats[i].AudioBitrate));
                    }
                }
                Debug.WriteLine("Quality: " + formats[i].Quality);
                Debug.WriteLine("Width: " + formats[i].Width);
                Debug.WriteLine("AudioBitrate: " + formats[i].AudioBitrate);
                Debug.WriteLine("Audiochannels: " + formats[i].AudioChannels);
                Debug.WriteLine("AudioCodec: " + formats[i].AudioCodec);
                Debug.WriteLine("VideoCodec: " + formats[i].VideoCodec);
                Debug.WriteLine("VideoBitrate: " + formats[i].VideoBitrate);
                Debug.WriteLine("Resolution: " + formats[i].Resolution);
            }

            audioResolutions.Sort();
            videoResolutions.Sort();

            return new VideoInfo(video.Title, video.WebpageUrl.ToString(), (int)video.Duration, video.Channel, video.Thumbnail, videoResolutions, audioResolutions);
        }

        public async Task<Video> AddToQueue(string url, DownloadOptions downloadOptions)
        {

            var res = await ytdl.RunVideoDataFetch(url);

            if (res.ErrorOutput != null || res.ErrorOutput.Any())
            {
                Debug.WriteLine("------------------------------------------------------------------");
                Debug.WriteLine($"Error output: {res.ErrorOutput[0]}");
            }
            // get some video information
            VideoData videoData = res.Data;

            return new Video(videoData.Title, videoData.WebpageUrl, (int)videoData.Duration, videoData.Channel, videoData.Thumbnail, downloadOptions.Filename, downloadOptions.OutputDir, downloadOptions.Format, downloadOptions.Starttime, downloadOptions.Endtime, downloadOptions.Resolution);
        }

        public async Task DownloadVideo(Video video)
        {

            OptionSet options;

            if (video.Format == "mp4" || video.Format == "mkv" || video.Format == "ogg" || video.Format == "webm")
            {
                DownloadMergeFormat videoFormat = DownloadMergeFormat.Unspecified;
                switch (video.Format)
                {
                    case "mp4":
                        videoFormat = DownloadMergeFormat.Mp4;
                        break;
                    case "webm":
                        videoFormat = DownloadMergeFormat.Webm;
                        break;
                    case "mkv":
                        videoFormat = DownloadMergeFormat.Mkv;
                        break;
                    case "ogg":
                        videoFormat = DownloadMergeFormat.Ogg;
                        break;
                }

                string resolution = video.Resolution.Replace("p", "");

                options = new OptionSet()
                {
                    Format = $"bestvideo[height<={resolution}]+bestaudio/best",
                    //AudioFormat = AudioConversionFormat.Best,
                    IgnoreErrors = false,
                    NoPlaylist = true,
                    Downloader = "m3u8:ffmpeg",
                    //DownloaderArgs = "ffmpeg:-nostats -loglevel 0",
                    Output = $"{video.Filename}.%(ext)s",
                    Paths = video.FilePath,
                    MergeOutputFormat = videoFormat,
                    RestrictFilenames = false,
                    ForceOverwrites = false,
                    NoOverwrites = true,
                    NoPart = true,
                    NoMtime = true,
                    FfmpegLocation = dir + @"\Downloadtools\ffmpeg.exe",
                    //Exec = "echo outfile: {}"
                };
            }
            else
            {
                AudioConversionFormat audioFormat = AudioConversionFormat.Best;

                switch (video.Format)
                {
                    case "mp3":
                        audioFormat = AudioConversionFormat.Mp3;
                        break;
                    case "wav":
                        audioFormat = AudioConversionFormat.Wav;
                        break;
                }

                Debug.WriteLine(video.Format + " und " + video.Resolution + " Format " + audioFormat);

                options = new OptionSet()
                {
                    AudioFormat = audioFormat,
                    //AudioQuality = byte.Parse(video.Resolution),
                    IgnoreErrors = false,
                    //IgnoreConfig = true,
                    Format = $"bestaudio[abr<={video.Resolution}]",
                    NoPlaylist = true,
                    Downloader = "m3u8:ffmpeg",
                    DownloaderArgs = "ffmpeg:-nostats -loglevel 0",
                    Output = $"{video.Filename}.%(ext)s",
                    Paths = video.FilePath,
                    RestrictFilenames = false,
                    ForceOverwrites = false,
                    KeepVideo = false,
                    NoOverwrites = true,
                    NoPart = true,
                    NoMtime = true,
                    FfmpegLocation = dir + @"\Downloadtools\ffmpeg.exe",
                    Exec = "echo outfile: {}",
                };
            }

            if (video.StartTime != 0 || video.EndTime < video.Duration)
            {
                TimeSpan start = TimeSpan.FromSeconds(video.StartTime);

                TimeSpan end = TimeSpan.FromSeconds(video.EndTime);

                Debug.WriteLine(start.ToString("hh':'mm':'ss"));
                Debug.WriteLine(end.ToString("hh':'mm':'ss"));

                options.PostprocessorArgs = new[]
                {
                    $"ffmpeg:-ss {start.ToString("hh':'mm':'ss")} -to {end.ToString("hh':'mm':'ss")}"
                };
            }

            // a progress handler with a callback that updates a progress bar
            var progress = new Progress<DownloadProgress>(p => Debug.WriteLine(p.Progress.ToString()));
            video.DownloadState = DOWNLOADING;

            RunResult<string> res = await ytdl.RunWithOptions(video.Url, options, progress: progress, ct: cts.Token);

            if (video.Format == "mp4" || video.Format == "mkv" || video.Format == "ogg" || video.Format == "webm")
            {
                res = await ytdl.RunWithOptions(video.Url, options, progress: progress, ct: cts.Token);
            }
            else
            {
                res = await ytdl.RunAudioDownload(video.Url, overrideOptions: options, progress: progress, ct: cts.Token);
            }

            Debug.WriteLine(res.Success);
            Debug.WriteLine(res.Data.ToString());

            if (res.ErrorOutput != null || res.ErrorOutput.Any())
            {
                Debug.WriteLine("-----------------hhhhhhhhhhh-------------------------------------------------");
                for (int i = 0; i<res.ErrorOutput.Length; i++) 
                {
                    Debug.WriteLine($"Error output: {res.ErrorOutput[i]}");
                }
            }

            if (!res.Success)
            {
                for (int i = 0; i < res.ErrorOutput.Length; i++)
                {
                    if (res.ErrorOutput[i].Contains("ERROR"))
                    {
                        if (res.ErrorOutput[i].Contains("[Errno 13]"))
                        {
                            throw new UnauthorizedAccessException(res.ErrorOutput[i].ToString());
                        }
                    }
                } 
            }
        }

        public void CancelDownload()
        {
            cts.Cancel();
            cts.Dispose();
            cts = new CancellationTokenSource();
        }
    }
}
