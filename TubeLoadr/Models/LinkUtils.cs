using System;

namespace TubeLoadr.Models
{
    internal class LinkUtils
    {
        /// <summary>
        /// Checks if a link is a valid youtube url.
        /// </summary>
        /// <param name="link">The youtube link.</param>
        /// <returns></returns>
        public static bool CheckIsYouTubeLink(string link)
        {
            if (string.IsNullOrEmpty(link)) return false;

            if (link.Contains("https://www.youtube") || link.Contains("https://youtu.be") && IsValidUrl(link))
            {
                return true;
            }

            return false;
        }

        private static bool IsValidUrl(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }
    }
}
