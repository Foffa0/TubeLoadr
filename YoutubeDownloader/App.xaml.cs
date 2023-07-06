using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;
using YoutubeDownloader.Stores;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {   
        private readonly NavigationStore _navigationStore;
        private readonly Downloader _downloader;

        public App()
        {   
            _downloader = new Downloader();
            _navigationStore = new NavigationStore();
        }

        protected override void OnStartup(StartupEventArgs e)
        {   
            _navigationStore.CurrentViewModel = CreateDownloadViewModel();

            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_navigationStore)
            };
            MainWindow.Show();

            base.OnStartup(e);
        }

        private DownloadViewModel CreateDownloadViewModel()
        {
            return new DownloadViewModel(_downloader, new NavigationService(_navigationStore, CreateAboutViewModel));
        }

        private ViewModelBase CreateAboutViewModel()
        {
            return new AboutViewModel();
        }
    }
}
