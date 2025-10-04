using Octokit;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using YoutubeDLSharp;

namespace TubeLoadr.Services.yt_dlp
{
    public class YtdlpAutoUpdate
    {
        private bool _isUpdating = false;
        public bool IsUpdating
        {
            get
            { return _isUpdating; }
            set
            {
                _isUpdating = value;
                IsUpdatingChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler IsUpdatingChanged;

        /// <summary>
        /// Checks if there is a newer release on Github
        /// </summary>
        /// <returns><see cref="bool"/> - True if there is a newer release.</returns>
        public async Task<bool> CheckGitHubNewerVersion()
        {
            bool versionState = false;
            GitHubClient client = new GitHubClient(new ProductHeaderValue("TubeLoadrApp"));

            var latest = await client.Repository.Release.GetLatest("yt-dlp", "yt-dlp");
            Version latestGitHubVersion = new Version(latest.TagName);

            string? dir = System.IO.Path.GetDirectoryName(Environment.ProcessPath);

            var ytdlp = new YoutubeDL();
            ytdlp.YoutubeDLPath = dir + @"\Downloadtools\yt-dlp.exe";

            Version localVersion = new Version(ytdlp.Version);
            Debug.WriteLine(ytdlp.Version);

            if (localVersion != latestGitHubVersion)
            {
                versionState = true;
            }

            return versionState;
        }

        public async Task DownloadYtdlp()
        {
            IsUpdating = true;
            string? dir = System.IO.Path.GetDirectoryName(Environment.ProcessPath);
            var ytdlp = new YoutubeDL();
            ytdlp.YoutubeDLPath = dir + @"\Downloadtools\yt-dlp.exe";
            await YoutubeDLSharp.Utils.DownloadYtDlp(dir + @"\Downloadtools\");
            IsUpdating = false;

            ytdlp = new YoutubeDL();
            ytdlp.YoutubeDLPath = dir + @"\Downloadtools\yt-dlp.exe";
            Debug.WriteLine("Updated to version: " + ytdlp.Version);
        }
    }
}
