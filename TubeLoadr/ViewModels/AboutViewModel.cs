using System.Diagnostics;
using System.Windows.Input;
using TubeLoadr.Commands;

namespace TubeLoadr.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public ICommand OpenGitHub { get; set; }

        public AboutViewModel()
        {
            OpenGitHub = new RelayCommand(o => OpenLink());
        }

        public void OpenLink()
        {
            ProcessStartInfo ProcessStartInfo = new ProcessStartInfo
            {
                FileName = "https://github.com/Foffa0/TubeLoadr",
                UseShellExecute = true
            };
            Process.Start(ProcessStartInfo);
        }
    }
}
