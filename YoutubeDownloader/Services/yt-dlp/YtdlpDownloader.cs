using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YoutubeDownloader.Models;
using YoutubeDownloader.Stores;

namespace YoutubeDownloader.Services.yt_dlp
{
    public class YtdlpDownloader
    {

        public async Task<Video> AddToQueue(string url)
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
                Debug.WriteLine(output);

                Dictionary<string, object> metadata;

                using (StreamReader r = new StreamReader($@"yt-dlp/{output}"))
                {
                    var json = r.ReadToEnd();
                    metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    Debug.WriteLine(metadata.GetValueOrDefault("title"));
                }
                //File.Delete($@"yt-dlp/{output}");


                JsonElement thumbnails = (JsonElement)metadata.GetValueOrDefault("thumbnails");

                string thumbnailUrl = "";

                foreach (var thumbnail in thumbnails.EnumerateArray())
                {
                    if (thumbnail.GetProperty("preference").ToString().Equals("0"))
                    {
                        thumbnailUrl = thumbnail.GetProperty("url").ToString();
                    }
                }

                Debug.Write(thumbnailUrl);


                Video video = new Video(metadata.GetValueOrDefault("title").ToString(), metadata.GetValueOrDefault("webpage_url").ToString(), int.Parse(metadata.GetValueOrDefault("duration").ToString()), metadata.GetValueOrDefault("channel").ToString(), thumbnailUrl);
                return video;
            });
            return video;
        }
    }
}
