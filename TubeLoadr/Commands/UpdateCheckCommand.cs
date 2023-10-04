﻿using System.Threading.Tasks;
using TubeLoadr.Services.GitHub;

namespace TubeLoadr.Commands
{
    internal class UpdateCheckCommand : AsyncCommandBase
    {
        private UpdateService _updateService;

        public UpdateCheckCommand(UpdateService updateService)
        {
            _updateService = updateService;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            bool updateAvailable = await _updateService.CheckGitHubNewerVersion();
            _updateService.IsNeverVersionAvailable = updateAvailable;
        }
    }
}
