using System;
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
        public int Duration { get; set; }
        public string Channel { get; set; }
        public string Thumbnail { get; set; }

        public string FilePath { get; set;}
        public string Format { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public string Resolution { get; set; }
    }
}
