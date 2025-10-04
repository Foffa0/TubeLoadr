using System;
using System.Windows;
using System.Windows.Input;
using TubeLoadr.Commands;
using TubeLoadr.Services;
using TubeLoadr.Services.GitHub;
using TubeLoadr.Services.yt_dlp;
using TubeLoadr.Stores;

namespace TubeLoadr.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;

        private readonly UpdateService _updateService;
        private readonly YtdlpAutoUpdate _ytdlpAutoUpdate;

        private string _windowStateSymbol;

        public string WindowStateSymbol
        {
            get { return _windowStateSymbol; }
            set
            {
                _windowStateSymbol = value;
                OnPropertyChanged(nameof(WindowStateSymbol));
            }
        }


        public ICommand DownloadCommand { get; }
        public ICommand DownloadHistoryCommand { get; }
        public ICommand AboutCommand { get; }
        public ICommand UpdateCheckCommand { get; }
        public ICommand OpenBrowserCommand { get; }

        public ICommand UpdateYtdlpCommand { get; }

        private bool newerVersionAvailable;
        private bool _isYtdlpUpdating;

        public bool NewerVersionAvailable
        {
            get { return newerVersionAvailable; }
            set
            {
                newerVersionAvailable = value;
                OnPropertyChanged(nameof(NewerVersionAvailable));
            }
        }
        public bool IsYtdlpUpdating
        {
            get { return _isYtdlpUpdating; }
            set
            {
                _isYtdlpUpdating = value;
                OnPropertyChanged(nameof(IsYtdlpUpdating));
            }
        }


        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

        public MainViewModel(NavigationStore navigationStore, NavigationService<DownloadViewModel> downloadNavigationService, NavigationService<DownloadHistoryViewModel> downloadHistoryNavigationService, NavigationService<AboutViewModel> aboutViewNavigationService)
        {
            _navigationStore = navigationStore;
            _updateService = new UpdateService();
            _updateService.IsNeverVersionAvailableChanged += OnUpdateAvailable;
            _ytdlpAutoUpdate = new YtdlpAutoUpdate();
            _ytdlpAutoUpdate.IsUpdatingChanged += OnYtdlpUpdateAvailable;

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            DownloadCommand = new NavigateCommand<DownloadViewModel>(downloadNavigationService);
            DownloadHistoryCommand = new NavigateCommand<DownloadHistoryViewModel>(downloadHistoryNavigationService);
            AboutCommand = new NavigateCommand<AboutViewModel>(aboutViewNavigationService);
            UpdateCheckCommand = new UpdateCheckCommand(_updateService);
            UpdateYtdlpCommand = new UpdateYtdlpCommand(_ytdlpAutoUpdate);
            OpenBrowserCommand = new OpenBrowserCommand();

            Application.Current.MainWindow.StateChanged += WindowStateChanged;

            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
            {
                WindowStateSymbol = "🗗";
            }
            else
            {
                WindowStateSymbol = "☐";
            }

            NewerVersionAvailable = false;

            UpdateCheckCommand.Execute(this);
            UpdateYtdlpCommand.Execute(this);
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        private void OnUpdateAvailable(object? sender, EventArgs e)
        {
            NewerVersionAvailable = _updateService.IsNeverVersionAvailable;
        }

        private void OnYtdlpUpdateAvailable(object? sender, EventArgs e)
        {
            IsYtdlpUpdating = _ytdlpAutoUpdate.IsUpdating;
        }

        private void WindowStateChanged(object? sender, EventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
            {
                WindowStateSymbol = "🗗";
            }
            else
            {
                WindowStateSymbol = "☐";
            }
        }

    }
}
