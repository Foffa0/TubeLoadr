using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using YoutubeDownloader.Exceptions;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.Services.yt_dlp
{
    public class YtdlpDownloader
    {
        public static string QUEUED = "Queued";
        public static string DOWNLOADING = "Downloading";
        public static string DOWNLOADED = "Finished";
        public static string ERROR = "Error";

        /// <summary>
        /// Get video metadata from url.
        /// </summary>
        /// <param name="url">The youtube video url.</param>
        /// <exception cref="VideoNotFoundException">Thrown if no video exists at the url.</exception>
        public async Task<VideoInfo> GetVideoInfo(string url)
        {
            ProcessStartInfo info = new ProcessStartInfo("cmd");
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardInput = true;
            info.CreateNoWindow = true;
            info.RedirectStandardError = true;
            info.WorkingDirectory = "D:/Downloads/yt-dlp";

            VideoInfo video = await Task.Run(async () =>
            {
                var proc = Process.Start(info);
                //This will give us the full name path of the executable file:
                //string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                //This will strip just the working path name:
                //string strWorkPath = Path.GetDirectoryName(strExeFilePath);

                string? dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                string file = dir + @"\yt-dlp";

                Debug.WriteLine(file);

                Directory.CreateDirectory(file);

                proc.StandardInput.WriteLine($"yt-dlp.exe --write-info-json -P {file} -o %(id)s.%(ext)s --skip-download {url}");

                proc.StandardInput.Close();
                ArgumentNullException.ThrowIfNull(proc);
                string output = proc.StandardOutput.ReadToEnd();
                await proc.WaitForExitAsync();
                Debug.WriteLine(output);

                string SearchStringStart = "Writing video metadata as JSON to:";
                string SearchStringEnd = ".info.json";

                if (!output.Contains(SearchStringStart))
                {
                    throw new VideoNotFoundException("Youtube video not found");
                }

                 if (output.Contains(SearchStringStart))
                {
                    int Start = output.IndexOf(SearchStringStart, 0) + SearchStringStart.Length;
                    int End = output.IndexOf(SearchStringEnd) + SearchStringEnd.Length;
                    Debug.WriteLine(Start);
                    output = output.Substring(Start, End - Start);

                    string SearchString = "yt-dlp";
                    int fileName = output.IndexOf(SearchString) + SearchString.Length + 1;
                    output = output.Substring(fileName);
                    output = output.TrimStart();
                }

                Dictionary<string, object> metadata;

                using (StreamReader r = new StreamReader($@"yt-dlp/{output}"))
                {
                    var json = r.ReadToEnd();
                    metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                }

                File.Delete($@"yt-dlp/{output}");


                JsonElement thumbnails = (JsonElement)metadata.GetValueOrDefault("thumbnails");

                JsonElement formats = (JsonElement)metadata.GetValueOrDefault("formats");

                string thumbnailUrl = "";

                foreach (var thumbnail in thumbnails.EnumerateArray())
                {
                    if (thumbnail.GetProperty("preference").ToString().Equals("0"))
                    {
                        thumbnailUrl = thumbnail.GetProperty("url").ToString();
                    }
                }

                // Get all available resolutions
                JsonElement resolution_option;
                var resolutionsVideo = new List<string> { "144p", "240p", "360p", "480p", "720p", "1080p", "1080p60", "1440p", "2160p" };

                List<string> availableResolutionsVideo = new();
                List<int> availableResolutionsAudio = new();

                foreach (var resolution in formats.EnumerateArray())
                {
                    if (resolution.TryGetProperty("format_note", out resolution_option))
                    {
                        string resolution_string = resolution_option.GetString();

                        // check if the resolution has a 60p tag and remove it to avoid duplicates
                        if (resolution_string.Contains("p60"))
                        {
                            resolution_string = resolution_string.Replace("p60", "p");
                        }

                        if (resolutionsVideo.Where(x => x.Contains(resolution_string)).FirstOrDefault() != null && availableResolutionsVideo.Where(x => x.Contains(resolution_string)).FirstOrDefault() == null)
                        {
                            availableResolutionsVideo.Add(resolution_string);
                        }
                    }
                    if (resolution.TryGetProperty("abr", out resolution_option))
                    {
                        if (resolution_option.GetDouble() > 0)
                        {
                            availableResolutionsAudio.Add((int)Math.Round(resolution_option.GetDouble(), 0, MidpointRounding.AwayFromZero));
                        }
                    }
                }
                availableResolutionsVideo.ForEach(p => Debug.WriteLine(p));

                availableResolutionsAudio.Sort();

                availableResolutionsAudio.ForEach(p => Debug.WriteLine(p));



                VideoInfo video = new VideoInfo(metadata.GetValueOrDefault("title").ToString(), metadata.GetValueOrDefault("webpage_url").ToString(), int.Parse(metadata.GetValueOrDefault("duration").ToString()), metadata.GetValueOrDefault("channel").ToString(), thumbnailUrl, availableResolutionsVideo, availableResolutionsAudio);
                return video;
            });
            return video;
        }

        public async Task<Video> AddToQueue(string url, DownloadOptions downloadOptions)
        {
            ProcessStartInfo info = new ProcessStartInfo("cmd");
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardInput = true;
            info.CreateNoWindow = true;
            info.RedirectStandardError = true;
            info.WorkingDirectory = "D:/Downloads/yt-dlp";

            Video video = await Task.Run(async () =>
            {
                var proc = Process.Start(info);
                //This will give us the full name path of the executable file:
                //string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                //This will strip just the working path name:
                //string strWorkPath = Path.GetDirectoryName(strExeFilePath);

                string? dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                string file = dir + @"\yt-dlp";

                Debug.WriteLine(file);

                Directory.CreateDirectory(file);

                proc.StandardInput.WriteLine($"yt-dlp.exe --write-info-json -P {file} -o %(id)s.%(ext)s --skip-download {url}");

                proc.StandardInput.Close();
                ArgumentNullException.ThrowIfNull(proc);
                string output = proc.StandardOutput.ReadToEnd();
                await proc.WaitForExitAsync();
                Debug.WriteLine(output);

                string SearchStringStart = "Writing video metadata as JSON to:";
                string SearchStringEnd = ".info.json";

                if (output.Contains(SearchStringStart))
                {
                    int Start = output.IndexOf(SearchStringStart, 0) + SearchStringStart.Length;
                    int End = output.IndexOf(SearchStringEnd) + SearchStringEnd.Length;
                    Debug.WriteLine(Start);
                    output = output.Substring(Start, End - Start);

                    string SearchString = "yt-dlp";
                    int fileName = output.IndexOf(SearchString) + SearchString.Length + 1;
                    output = output.Substring(fileName);
                    output = output.TrimStart();
                }

                Dictionary<string, object> metadata;

                using (StreamReader r = new StreamReader($@"yt-dlp/{output}"))
                {
                    var json = r.ReadToEnd();
                    metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                }
                File.Delete($@"yt-dlp/{output}");


                JsonElement thumbnails = (JsonElement)metadata.GetValueOrDefault("thumbnails");

                JsonElement formats = (JsonElement)metadata.GetValueOrDefault("formats");

                string thumbnailUrl = "";

                foreach (var thumbnail in thumbnails.EnumerateArray())
                {
                    if (thumbnail.GetProperty("preference").ToString().Equals("0"))
                    {
                        thumbnailUrl = thumbnail.GetProperty("url").ToString();
                    }
                }

                Debug.Write(thumbnailUrl);


                Video video = new Video(metadata.GetValueOrDefault("title").ToString(), metadata.GetValueOrDefault("webpage_url").ToString(), int.Parse(metadata.GetValueOrDefault("duration").ToString()), metadata.GetValueOrDefault("channel").ToString(), thumbnailUrl, downloadOptions.Filename, downloadOptions.OutputDir, downloadOptions.Format, downloadOptions.Starttime, downloadOptions.Endtime, downloadOptions.Resolution);
                return video;
            });
            return video;
        }

        public async Task DownloadVideo(Video video)
        {

            ProcessStartInfo info = new ProcessStartInfo("cmd");
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardInput = true;
            info.CreateNoWindow = true;
            info.RedirectStandardError = true;
            info.WorkingDirectory = "D:/Downloads/yt-dlp";

            video.DownloadState = DOWNLOADING;
            
            await Task.Run(async () =>
            {
                var proc = Process.Start(info);
                proc.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                proc.BeginOutputReadLine();

                string formatOptions = "";
                string resolutionOptions = "";
                string posprocessorArgs = "";

                if (video.Format == "mp4" || video.Format == "mkv" || video.Format == "ogg" || video.Format =="webm")
                {
                    formatOptions = $"--merge-output-format {video.Format}";//$"--recode-video {vid.Format}";
                    string resolution = video.Resolution.Replace("p", "");
                    resolutionOptions = $""" "-f bestvideo[height<={resolution}]+bestaudio/best" """;
                }
                else
                {
                    formatOptions = $"-x --audio-format {video.Format}";
                    resolutionOptions = $"--audio-quality 128k";
                }

                if (video.StartTime != 0 || video.EndTime < video.Duration)
                {
                    TimeSpan start = TimeSpan.FromSeconds(video.StartTime);

                    TimeSpan end = TimeSpan.FromSeconds(video.EndTime);

                    posprocessorArgs = $"""--postprocessor-args "-ss {start.ToString("hh':'mm':'ss")} -to {end}" """;
                }

                proc.StandardInput.WriteLine($""""yt-dlp.exe --ffmpeg-location D:/Downloads/yt-dlp --prefer-ffmpeg --no-mtime {formatOptions} {resolutionOptions} {posprocessorArgs} -o "{video.Filename}.%(ext)s" -P {video.FilePath} {video.Url}"""");

                proc.StandardInput.Close();
                ArgumentNullException.ThrowIfNull(proc);
                //string output = proc.StandardOutput.ReadToEnd();
                //Debug.WriteLine(output);
                await proc.WaitForExitAsync();

            });
        }

        static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            Debug.WriteLine(outLine.Data);
        }
    }
}
