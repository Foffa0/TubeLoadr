using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;
using TubeLoadr.Commands;

namespace TubeLoadr.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        private string _version;

        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public ICommand OpenGitHub { get; set; }

        public AboutViewModel()
        {
            Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
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
