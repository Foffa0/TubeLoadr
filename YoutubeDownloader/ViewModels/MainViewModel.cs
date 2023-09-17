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

        public MainViewModel(NavigationStore navigationStore, NavigationService<DownloadViewModel> downloadNavigationService, NavigationService<DownloadHistoryViewModel> downloadHistoryNavigationService, NavigationService<AboutViewModel> aboutViewNavigationService)
        {
            _navigationStore = navigationStore;

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            DownloadCommand = new NavigateCommand<DownloadViewModel>(downloadNavigationService);
            DownloadHistoryCommand = new NavigateCommand<DownloadHistoryViewModel>(downloadHistoryNavigationService);
            AboutCommand = new NavigateCommand<AboutViewModel>(aboutViewNavigationService);
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
