using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
