using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.Stores;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader.Commands
{
    internal class NavigateCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<ViewModelBase> _createViewModel;

        public NavigateCommand(NavigationStore navigationStore, Func<ViewModelBase> createViewModel)
        {
            _navigationStore = navigationStore;
            _createViewModel = createViewModel;
        }

        public override void Execute(object? parameter)
        {
            _navigationStore.CurrentViewModel = _createViewModel();
        }
    }
}
