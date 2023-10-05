using System;
using System.Windows;
using System.Windows.Input;
using TubeLoadr.Commands;
using TubeLoadr.Services;
using TubeLoadr.Services.GitHub;
using TubeLoadr.Stores;

namespace TubeLoadr.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;

        private readonly UpdateService _updateService;

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

        private bool neverVersionAvailable;

        public bool NewerVersionAvailable
        {
            get { return neverVersionAvailable; }
            set
            {
                neverVersionAvailable = value;
                OnPropertyChanged(nameof(NewerVersionAvailable));
            }
        }


        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

        public MainViewModel(NavigationStore navigationStore, NavigationService<DownloadViewModel> downloadNavigationService, NavigationService<DownloadHistoryViewModel> downloadHistoryNavigationService, NavigationService<AboutViewModel> aboutViewNavigationService)
        {
            _navigationStore = navigationStore;
            _updateService = new UpdateService();
            _updateService.IsNeverVersionAvailableChanged += OnUpdateAvailable;

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            DownloadCommand = new NavigateCommand<DownloadViewModel>(downloadNavigationService);
            DownloadHistoryCommand = new NavigateCommand<DownloadHistoryViewModel>(downloadHistoryNavigationService);
            AboutCommand = new NavigateCommand<AboutViewModel>(aboutViewNavigationService);
            UpdateCheckCommand = new UpdateCheckCommand(_updateService);
            OpenBrowserCommand = new OpenBrowserCommand();

            Application.Current.MainWindow.StateChanged += WindowStateChanged;

            NewerVersionAvailable = false;

            UpdateCheckCommand.Execute(this);
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        private void OnUpdateAvailable(object? sender, EventArgs e)
        {
            NewerVersionAvailable = _updateService.IsNeverVersionAvailable;
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
