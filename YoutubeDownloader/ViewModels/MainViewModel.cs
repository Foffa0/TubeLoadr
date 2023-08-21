using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YoutubeDownloader.Commands;
using YoutubeDownloader.Services;
using YoutubeDownloader.Stores;

namespace YoutubeDownloader.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;

        public ICommand DownloadCommand { get; }
        public ICommand DownloadHistoryCommand { get; }
        public ICommand AboutCommand { get; }

        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

        public MainViewModel(NavigationStore navigationStore, NavigationService downloadNavigationService, NavigationService downloadHistoryNavigationService, NavigationService aboutViewNavigationService)
        {
            _navigationStore = navigationStore;

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            DownloadCommand = new NavigateCommand(downloadNavigationService);
            DownloadHistoryCommand = new NavigateCommand(downloadHistoryNavigationService);
            AboutCommand = new NavigateCommand(aboutViewNavigationService);
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
