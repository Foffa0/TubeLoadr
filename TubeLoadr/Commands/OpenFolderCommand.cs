using System.Diagnostics;

namespace TubeLoadr.Commands
{
    class OpenFolderCommand : CommandBase
    {
        public override void Execute(object? parameter)
        {
            Process.Start("explorer.exe", $@"{parameter}");
        }
    }
}
