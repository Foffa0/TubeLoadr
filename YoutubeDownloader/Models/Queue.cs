using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YoutubeDownloader.Models
{
    internal class Queue
    {
        private readonly Queue<Video> _video_queue;
        private readonly Queue<Video> _finished_queue;

        public Queue() 
        {
            _video_queue = new Queue<Video>();
            _finished_queue = new Queue<Video>();
        }

        public IEnumerable<Video> GetVideoQueue()
        {
            return _video_queue.ToArray();
        }

        public void AddVideo(Video video)
        {
            _video_queue.Enqueue(video);
        }

        public Video NextVideo()
        {
            return _video_queue.Peek();
        }

        public void Mark_Video_Downloaded()
        {
            _finished_queue.Enqueue(_video_queue.Dequeue());
        }

        public void Clear_Queue()
        {
            _video_queue.Clear();
            _finished_queue.Clear();
        }
    }
}
