using YoutubeDownloader.Models;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader.Commands
{
    internal class StartStopDownloadCommand : CommandBase
    {
        private DownloadViewModel _downloadViewModel;
        private Downloader _downloader;

        public StartStopDownloadCommand(DownloadViewModel viewmodel, Downloader downloader) 
        {
            _downloadViewModel = viewmodel;
            _downloader = downloader;
        }
        public override void Execute(object? parameter)
        {
            if (_downloadViewModel.IsDownloading)
            {
                _downloader.CancelDownload();
            }
            else
            {
               _downloader.DownloadVideo();
            }
            _downloadViewModel.LoadQueuedVideosCommand.Execute(null);
        }
    }
}
