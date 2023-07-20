using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.Models
{
    internal class LinkUtils
    {
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
