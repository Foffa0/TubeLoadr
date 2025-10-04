using System.Threading.Tasks;
using TubeLoadr.Services.yt_dlp;

namespace TubeLoadr.Commands
{
    internal class UpdateYtdlpCommand : AsyncCommandBase
    {
        private YtdlpAutoUpdate _updateService;

        public UpdateYtdlpCommand(YtdlpAutoUpdate updateService)
        {
            _updateService = updateService;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            bool updateAvailable = await _updateService.CheckGitHubNewerVersion();
            if (updateAvailable)
            {
                await _updateService.DownloadYtdlp();
            }
        }
    }
}
