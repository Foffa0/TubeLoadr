using Microsoft.VisualBasic;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YoutubeDownloader.Commands;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;
using YoutubeDownloader.Stores;

namespace YoutubeDownloader.ViewModels
{
    class DownloadViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly DownloaderStore _downloaderStore;

        //Video infos
        private string _videoUrl;
        public string VideoUrl
        {
            get
            {
                return _videoUrl;
            }
            set
            {
                _videoUrl = value;

                ClearErrors(nameof(VideoUrl));

                if (!LinkUtils.CheckIsYouTubeLink(value) && !string.IsNullOrEmpty(value))
                {
                    AddError("Invalid link! Only YouTube links are allowed.", nameof(VideoUrl));
                }

                OnPropertyChanged(nameof(VideoUrl));
            }
        }

        private string _outputDir;
        public string OutputDir
        {
            get { return _outputDir; }
            set
            {
                _outputDir = value;
                OnPropertyChanged(nameof(OutputDir));
            }
        }


        private readonly ObservableCollection<VideoViewModel> _videos;

        public IEnumerable<VideoViewModel> Videos => _videos;

        private string _errorMessage;
        public string ErrorMessage
        {
            get 
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value; 
                OnPropertyChanged(nameof(ErrorMessage));
                OnPropertyChanged(nameof(HasErrorMessage));
            }

        }

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        private readonly Dictionary<string, List<string>> _propertyNameToErrorsDictionary;

        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public ICommand DownloadCommand { get; }
        public ICommand DownloadHistoryCommand { get; }
        public ICommand AboutCommand { get; }
        public ICommand LoadQueuedVideosCommand { get; }
        public ICommand CommonOpenFileDialogCommand { get; }

        public DownloadViewModel(DownloaderStore downloaderStore, NavigationService aboutViewNavigationService, NavigationService downloadHistoryNavigationService)
        {
            _downloaderStore = downloaderStore;

            DownloadCommand = new DownloadCommand(_downloaderStore, _downloaderStore.Downloader, this);
            DownloadHistoryCommand = new NavigateCommand(downloadHistoryNavigationService);
            AboutCommand = new NavigateCommand(aboutViewNavigationService);
            CommonOpenFileDialogCommand = new RelayCommand(o => SelectOutputFolder());

            LoadQueuedVideosCommand = new LoadQueuedVideosCommand(this, downloaderStore);

            _videos = new ObservableCollection<VideoViewModel>();

            _downloaderStore.QueuedVideoCreated += OnVideoCreated;
            _downloaderStore.QueuedVideoDeleted += OnVideoDeleted;

            _propertyNameToErrorsDictionary = new Dictionary<string, List<string>>();

            _outputDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }


        public override void Dispose()
        {
            _downloaderStore.QueuedVideoCreated -= OnVideoCreated;
            _downloaderStore.QueuedVideoDeleted -= OnVideoDeleted;
            base.Dispose();
        }

        public static DownloadViewModel LoadViewModel(DownloaderStore downloaderStore, NavigationService downloadViewNavigationService, NavigationService aboutViewNavigationService)
        {
            DownloadViewModel viewModel = new DownloadViewModel(downloaderStore, aboutViewNavigationService, downloadViewNavigationService);
            viewModel.LoadQueuedVideosCommand.Execute(null);
            return viewModel;
        }


        private void OnVideoCreated(Video video)
        {
            VideoViewModel videoViewModel = new VideoViewModel(video);
            _videos.Add(videoViewModel);
        }
        
        private void OnVideoDeleted(Video video)
        {
            VideoViewModel videoViewModel = new VideoViewModel(video);
            _videos.Remove(_videos.Where(i => i.Id == video.Id).Single());
        }


        public void UpdateVideos(IEnumerable<Video> videos)
        {
            _videos.Clear();

            foreach (Video video in videos)
            {
                VideoViewModel v = new VideoViewModel(video);
                _videos.Add(v);
            }
        }


        // Download options

        public void SelectOutputFolder()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.IsFolderPicker = true;
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                _outputDir = dialog.FileName;
                OnPropertyChanged(nameof(OutputDir));
            }
        }


        // Input validation

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public bool HasErrors => _propertyNameToErrorsDictionary.Any();

        public IEnumerable GetErrors(string? propertyName)
        {
            return _propertyNameToErrorsDictionary.GetValueOrDefault(propertyName, new List<string>());
        }

        private void AddError(string errorMessage, string propertyName)
        {
            if (!_propertyNameToErrorsDictionary.ContainsKey(propertyName))
            {
                _propertyNameToErrorsDictionary.Add(propertyName, new List<string>());
            }

            _propertyNameToErrorsDictionary[propertyName].Add(errorMessage);

            OnErrorsChanged(propertyName);
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void ClearErrors(string propertyName)
        {
            _propertyNameToErrorsDictionary.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }
    }
}
