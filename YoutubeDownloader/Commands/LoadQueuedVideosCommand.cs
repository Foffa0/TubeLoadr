using System;
using System.Threading.Tasks;
using YoutubeDownloader.Stores;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader.Commands
{
    internal class LoadQueuedVideosCommand : AsyncCommandBase
    {
        private readonly DownloadViewModel _viewModel;
        private readonly DownloaderStore _downloaderStore;

        public LoadQueuedVideosCommand(DownloadViewModel viewModel, DownloaderStore downloaderStore)
        {
            _viewModel = viewModel;
            _downloaderStore = downloaderStore;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            _viewModel.ErrorMessage = string.Empty;
            _viewModel.IsLoading = true;
            try
            {
                await _downloaderStore.Load();
                _viewModel.UpdateVideos(_downloaderStore.DownloadQueue);

            }
            catch (Exception e)
            {
                _viewModel.ErrorMessage = "Failed to load videos.";
            }

            _viewModel.IsLoading = false;
        }
    }
}
