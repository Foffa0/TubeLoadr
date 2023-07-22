using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.Models
{
    public class DownloadOptions
    {
        public string OutputDir;

        public DownloadOptions(string outputDir)
        {
            OutputDir = outputDir;
        }
    }
}
