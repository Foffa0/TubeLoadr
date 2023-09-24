using System.Linq;
using System.Threading.Tasks;
using TubeLoadr.Models;
using TubeLoadr.Services.yt_dlp;
using TubeLoadr.Stores;
using TubeLoadr.ViewModels;

namespace TubeLoadr.Commands
{
    internal class RemoveFromQueueCommand : AsyncCommandBase
    {
        private DownloaderStore _downloaderStore;
        private Downloader _downloader;

        public RemoveFromQueueCommand(DownloaderStore downloaderStore, Downloader downloader)
        {
            _downloaderStore = downloaderStore;
            _downloader = downloader;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            if (((VideoViewModel)parameter).DownloadState == YtdlpDownloader.DOWNLOADING)
            {
                _downloader.CancelDownload();
                await _downloaderStore.DeleteQueuedVideo(_downloaderStore.DownloadQueue.Where(i => i.Id == ((VideoViewModel)parameter).Id).Single());
                _downloader.DownloadVideo();
            }
            else
            {
                await _downloaderStore.DeleteQueuedVideo(_downloaderStore.DownloadQueue.Where(i => i.Id == ((VideoViewModel)parameter).Id).Single());
            }

        }
    }
}
