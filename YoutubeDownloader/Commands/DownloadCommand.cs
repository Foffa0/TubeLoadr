using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader.Commands
{
    class DownloadCommand : CommandBase
    {
        private readonly DownloadViewModel _downloadViewModel;

        public DownloadCommand(DownloadViewModel downloadViewModel) 
        {
            _downloadViewModel = downloadViewModel;

            _downloadViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }


        public override bool CanExecute(object? parameter)
        {
            return !string.IsNullOrEmpty(_downloadViewModel.VideoUrl) && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            //TODO: 
        }


        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DownloadViewModel.VideoUrl))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
