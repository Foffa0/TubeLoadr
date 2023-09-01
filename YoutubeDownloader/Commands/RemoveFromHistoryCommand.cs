using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YoutubeDownloader.Stores;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader.Commands
{
    class RemoveFromHistoryCommand : AsyncCommandBase
    {

        private readonly DownloadHistoryViewModel _viewModel;
        private readonly DownloaderStore _downloaderStore;

        public RemoveFromHistoryCommand(DownloadHistoryViewModel viewModel, DownloaderStore downloaderStore)
        {
            _viewModel = viewModel;
            _downloaderStore = downloaderStore;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            Debug.WriteLine(parameter.ToString());
            
            await _downloaderStore.DeleteDownloadedVideo(_downloaderStore.Videos.Where(i => i.Id == ((DownloadedVideoViewModel)parameter).Id).Single());
        }
    }
}
