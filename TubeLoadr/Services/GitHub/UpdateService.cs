using Octokit;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace TubeLoadr.Services.GitHub
{
    public class UpdateService
    {
        private bool _isNeverVersionAvailable;
        public bool IsNeverVersionAvailable
        {
            get
            { return _isNeverVersionAvailable; }
            set
            {
                _isNeverVersionAvailable = value;
                IsNeverVersionAvailableChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler IsNeverVersionAvailableChanged;

        /// <summary>
        /// Checks if there is a newer release on Github
        /// </summary>
        /// <returns><see cref="bool"/> - True if there is a newer release.</returns>
        public async Task<bool> CheckGitHubNewerVersion()
        {
            bool versionState = false;
            GitHubClient client = new GitHubClient(new ProductHeaderValue("TubeLoadrApp"));

            int index = Assembly.GetEntryAssembly().GetName().Version.ToString().LastIndexOf(".");

            Version localVersion = new Version(Assembly.GetEntryAssembly().GetName().Version.ToString().Substring(0, index));
            var latest = await client.Repository.Release.GetLatest("Foffa0", "TubeLoadr");
            Version latestGitHubVersion = new Version(latest.TagName.Substring(1));

            //Compare the Versions
            int versionComparison = localVersion.CompareTo(latestGitHubVersion);
            if (versionComparison < 0)
            {
                //The version on GitHub is more up to date than this local release.
                Debug.WriteLine("Never Version avail");
                versionState = true;
            }
            else if (versionComparison > 0)
            {
                //This local version is greater than the release version on GitHub.
                Debug.WriteLine("This local version is greater than the release version on GitHub.");
            }
            else
            {
                //This local Version and the Version on GitHub are equal.
                Debug.WriteLine("This local Version and the Version on GitHub are equal.");
            }
            return versionState;
        }
    }
}
