using System.Diagnostics;

namespace YoutubeDownloader.Commands
{
    class OpenFolderCommand : CommandBase
    {
        public override void Execute(object? parameter)
        {
            Process.Start("explorer.exe", $@"{parameter}");
        }
    }
}
