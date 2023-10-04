using System.Diagnostics;

namespace TubeLoadr.Commands
{
    /// <summary>
    /// Opens a given url (through the CommandParameter) in the default browser.
    /// </summary>
    internal class OpenBrowserCommand : CommandBase
    {
        public override void Execute(object? parameter)
        {
            ProcessStartInfo ProcessStartInfo = new ProcessStartInfo
            {
                FileName = (string)parameter,
                UseShellExecute = true
            };
            Process.Start(ProcessStartInfo);
        }
    }
}
