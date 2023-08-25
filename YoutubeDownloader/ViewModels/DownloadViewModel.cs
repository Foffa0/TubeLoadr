using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Windows.Input;
using YoutubeDownloader.Commands;
using YoutubeDownloader.Models;
using YoutubeDownloader.Stores;

namespace YoutubeDownloader.ViewModels
{
    class DownloadViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly DownloaderStore _downloaderStore;

    // Download options
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
                if (string.IsNullOrEmpty(value))
                {
                    VideoTemp = null;
                    OnPropertyChanged(nameof(VideoTemp));
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

        private List<string> _availableFormats = new() { "mp3", "wav", "mp4", "webm", "ogg", "mkv" };
        public List<string> AvailableFormats
        { get { return _availableFormats; } }


        private string _format;
        public string Format
        {
            get { return _format; }
            set
            {
                _format = value;
                OnPropertyChanged(nameof(Format));

                if (_format == "mp3" || _format == "wav")
                {
                    _isVideoFormat = false;
                    _reslutionVideo = string.Empty;
                    OnPropertyChanged(nameof(ResolutionVideo));
                } 
                else
                {
                    _isVideoFormat = true;
                    _reslutionAudio = null;
                    OnPropertyChanged(nameof(ResolutionAudio));
                }
                OnPropertyChanged(nameof(IsVideoFormat));
            }
        }

        private bool _isVideoFormat;
        public bool IsVideoFormat
        {
            get { return _isVideoFormat; }
            set 
            {
                _isVideoFormat = value;
                OnPropertyChanged(nameof(IsVideoFormat));
            }
        }

        private int _timestampStart;
        public int TimestampStart
        {
            get { return _timestampStart; }
            set
            {
                _timestampStart = value;
                OnPropertyChanged(nameof(TimestampStart));
            }
        }

        private int _timestampEnd;
        public int TimestampEnd
        {
            get { return _timestampEnd; }
            set
            {
                _timestampEnd = value;
                OnPropertyChanged(nameof(TimestampEnd));
            }
        }

        private int _videoLength;
        public int VideoLength
        {
            get { return _videoLength; }
            set
            {
                _videoLength = value;
                OnPropertyChanged(nameof(VideoLength));
            }
        }

        private List<string> _availableResolutionsVideo;
        public List<string> AvailableResolutionsVideo
        { get { return _availableResolutionsVideo; } }

        private string _reslutionVideo;
        public string ResolutionVideo
        {
            get { return _reslutionVideo; }
            set
            {
                _reslutionVideo = value;
                OnPropertyChanged(nameof(ResolutionVideo));
            }
        }

        private List<int> _availableResolutionsAudio;
        public List<int> AvailableResolutionsAudio
        { get { return _availableResolutionsAudio; } }

        private int? _reslutionAudio;
        public int? ResolutionAudio
        {
            get { return _reslutionAudio; }
            set
            {
                _reslutionAudio = value;
                OnPropertyChanged(nameof(ResolutionAudio));
            }
        }

        private string _filename;
        public string Filename
        {
            get { return _filename; }
            set
            {
                _filename = value;
                OnPropertyChanged(nameof(Filename));
            }
        }

        // Video that belongs to the currently entered url (before pressing download btn)
        private VideoInfo? _videoTemp;
        public VideoInfo? VideoTemp
        {
            get
            {
                return _videoTemp;
            }
            set
            {
                _videoTemp = value;
                OnPropertyChanged(nameof(VideoTemp));
            }
        }

        private bool _isLoadingVideoTemp;
        public bool IsLoadingVideoTemp
        {
            get { return _isLoadingVideoTemp; }
            set { _isLoadingVideoTemp = value; OnPropertyChanged(nameof(IsLoadingVideoTemp));}
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
        public ICommand LoadQueuedVideosCommand { get; }
        public ICommand CommonOpenFileDialogCommand { get; }
        public ICommand CmbFormats_SelectionChangedCommand { get; }

        public DownloadViewModel(DownloaderStore downloaderStore)
        {
            _downloaderStore = downloaderStore;

            DownloadCommand = new DownloadCommand(_downloaderStore, _downloaderStore.Downloader, this);

            CommonOpenFileDialogCommand = new RelayCommand(o => SelectOutputFolder());

            LoadQueuedVideosCommand = new LoadQueuedVideosCommand(this, downloaderStore);

            _videos = new ObservableCollection<VideoViewModel>();

            _downloaderStore.QueuedVideoCreated += OnVideoCreated;
            _downloaderStore.QueuedVideoDeleted += OnVideoDeleted;

            _propertyNameToErrorsDictionary = new Dictionary<string, List<string>>();

            _outputDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _timestampStart = 10;
            IsVideoFormat = true;
            _videoTemp = null;
            OnPropertyChanged(nameof(VideoTemp));
            _isLoadingVideoTemp = false;

            this.PropertyChanged += ViewModel_PropertyChanged;
        }

        private async void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "VideoUrl":
                    if (((IEnumerable<string>)GetErrors(nameof(VideoUrl))).Any() || VideoUrl.Length < 1) 
                    {
                        VideoTemp = null;
                        return;
                    }

                    _isLoadingVideoTemp = true;
                    OnPropertyChanged(nameof(IsLoadingVideoTemp));

                    VideoTemp = await _downloaderStore.Downloader.GetVideoInfo(_videoUrl);
                    OnPropertyChanged(nameof(VideoTemp));
                    _timestampStart = 0;
                    OnPropertyChanged(nameof(TimestampStart));
                    _timestampEnd = _videoTemp.Duration;
                    OnPropertyChanged(nameof(TimestampEnd));
                    _videoLength = _videoTemp.Duration;
                    OnPropertyChanged(nameof(VideoLength));
                    _availableResolutionsVideo = _videoTemp.AvailableResolutionsVideo;
                    OnPropertyChanged(nameof(AvailableResolutionsVideo));
                    _availableResolutionsAudio = _videoTemp.AvailableResolutionsAudio;
                    OnPropertyChanged(nameof(AvailableResolutionsAudio));
                    _filename = _videoTemp.Title; 
                    OnPropertyChanged(nameof(Filename));

                    _isLoadingVideoTemp = false;
                    OnPropertyChanged(nameof(IsLoadingVideoTemp));
                    break;
            }
        }

        public override void Dispose()
        {
            _downloaderStore.QueuedVideoCreated -= OnVideoCreated;
            _downloaderStore.QueuedVideoDeleted -= OnVideoDeleted;
            base.Dispose();
        }

        public static DownloadViewModel LoadViewModel(DownloaderStore downloaderStore)
        {
            DownloadViewModel viewModel = new DownloadViewModel(downloaderStore);
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
