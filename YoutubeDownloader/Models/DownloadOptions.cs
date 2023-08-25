﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.Models
{
    public class DownloadOptions
    {
        public string Filename;
        public string OutputDir;
        public string Format;
        public int Starttime;
        public int Endtime;
        public string Resolution;

        public DownloadOptions(string filename, string outputDir, string format, int startTime, int endTime, string resolution)
        {
            Filename = filename;
            OutputDir = outputDir;
            Format = format;
            Starttime = startTime;
            Endtime = endTime;
            Resolution = resolution;
        }
    }
}
