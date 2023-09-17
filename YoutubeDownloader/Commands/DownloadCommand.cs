using System.ComponentModel;
using System.Configuration;
using System.Threading.Tasks;
using YoutubeDownloader.Models;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader.Commands
{
    public class DownloadCommand : AsyncCommandBase
    {
        private readonly Downloader _downloader;
        private readonly DownloadViewModel _downloadViewModel;
        private readonly Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        public DownloadCommand(Downloader downloader, DownloadViewModel downloadViewModel)
        {
            _downloader = downloader;

            _downloadViewModel = downloadViewModel;
            _downloadViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }


        public override bool CanExecute(object? parameter)
        {
            return !string.IsNullOrEmpty(_downloadViewModel.VideoUrl) && !_downloadViewModel.HasErrors && !string.IsNullOrEmpty(_downloadViewModel.Format) && !string.IsNullOrEmpty(_downloadViewModel.Filename) && (_downloadViewModel.ResolutionAudio != null || !string.IsNullOrEmpty(_downloadViewModel.ResolutionVideo)) && !string.IsNullOrEmpty(_downloadViewModel.OutputDir) && base.CanExecute(parameter);
        }

        public override async Task ExecuteAsync(object parameter)
        {
            /*try
            {*/
            _downloadViewModel.IsLoadingAddToQueue = true;
            if (!AppConfig.AppSettings.Settings["downloadDirectory"].Value.Equals(_downloadViewModel.OutputDir))
            {
                AppConfig.AppSettings.Settings["downloadDirectory"].Value = _downloadViewModel.OutputDir;
                AppConfig.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(AppConfig.AppSettings.SectionInformation.Name);
            }

            string resolution = _downloadViewModel.IsVideoFormat ? _downloadViewModel.ResolutionVideo : _downloadViewModel.ResolutionAudio.ToString();

            DownloadOptions downloadOptions = new DownloadOptions(_downloadViewModel.Filename, _downloadViewModel.OutputDir, _downloadViewModel.Format, _downloadViewModel.TimestampStart, _downloadViewModel.TimestampEnd, resolution);

            await _downloader.GetVideoInfoAndAddToQueue(_downloadViewModel.VideoUrl, downloadOptions);
            _downloadViewModel.VideoUrl = string.Empty;
            _downloadViewModel.Filename = string.Empty;
            _downloadViewModel.VideoTemp = null;

            _downloadViewModel.IsLoadingAddToQueue = false;
            /*}
            catch (Exception) 
            {
                MessageBox.Show("Failed to save Video.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            */
        }


        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DownloadViewModel.VideoUrl) || e.PropertyName == nameof(DownloadViewModel.Format) || e.PropertyName == nameof(DownloadViewModel.ResolutionVideo) || e.PropertyName == nameof(DownloadViewModel.ResolutionAudio) || e.PropertyName == nameof(DownloadViewModel.OutputDir) || e.PropertyName == nameof(DownloadViewModel.Filename))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
