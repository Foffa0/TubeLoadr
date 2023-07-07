﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.DTOs
{
    public class VideoDTO
    {
        [Key]
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Url { get; set; }
        public string Duration { get; set; }
        public string Channel { get; set; }
        public string Thumbnail { get; set; }
    }
}
