using System;
using System.Runtime.Serialization;

namespace YoutubeDownloader.Exceptions
{
    internal class VideoNotFoundException : Exception
    {
        public VideoNotFoundException()
        {
        }

        public VideoNotFoundException(string? message) : base(message)
        {
        }

        public VideoNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected VideoNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
