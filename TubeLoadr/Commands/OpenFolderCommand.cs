using System.Diagnostics;

namespace TubeLoadr.Commands
{
    class OpenFolderCommand : CommandBase
    {
        /// <summary>
        /// Open a folder in win file explorer
        /// </summary>
        /// <param name="parameter"></param>
        public override void Execute(object? parameter)
        {
            Process.Start("explorer.exe", $@"{parameter}");
        }
    }
}
