using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services.yt_dlp;
using YoutubeDownloader.Stores;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader.Commands
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
