using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using YoutubeDownloader.DbContexts;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;
using YoutubeDownloader.Services.VideoCreators;
using YoutubeDownloader.Services.VideoProviders;
using YoutubeDownloader.Services.yt_dlp;
using YoutubeDownloader.Stores;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string CONNECTION_STRING = "Data Source=downloader.db";

        private readonly NavigationStore _navigationStore;
        private readonly Downloader _downloader;
        private readonly DownloaderStore _downloaderStore;

        private readonly DownloaderDbContextFactory _downloaderDbContextFactory;

        public App()
        {
            _downloaderDbContextFactory = new DownloaderDbContextFactory(CONNECTION_STRING);
            IVideoCreator videoCreator = new DatabaseVideoCreator(_downloaderDbContextFactory);
            IVideoProvider videoProvider = new DatabaseVideoProvider(_downloaderDbContextFactory);

            
            _downloaderStore = new DownloaderStore(videoProvider, videoCreator);
            _downloader = new Downloader(_downloaderStore);
            _navigationStore = new NavigationStore();
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            using (DownloaderDbContext dbContext = _downloaderDbContextFactory.CreateDbContext())
            {
                dbContext.Database.Migrate();
            }


            _navigationStore.CurrentViewModel = CreateDownloadViewModel();

            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_navigationStore, new NavigationService(_navigationStore, CreateDownloadViewModel), new NavigationService(_navigationStore, CreateDownloadHistoryViewModel), new NavigationService(_navigationStore, CreateAboutViewModel))
            };
            MainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _downloader.CancelDownload();

            base.OnExit(e);
        }

        private DownloadViewModel CreateDownloadViewModel()
        {
            return DownloadViewModel.LoadViewModel(_downloaderStore, _downloader);
        }

        private DownloadHistoryViewModel CreateDownloadHistoryViewModel()
        {
            return DownloadHistoryViewModel.LoadViewModel(_downloaderStore, new NavigationService(_navigationStore, CreateDownloadViewModel));
        }

        private ViewModelBase CreateAboutViewModel()
        {
            return new AboutViewModel();
        }
    }
}
