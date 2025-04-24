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

        public ICommand OpenBrowserCommand { get; set; }

        public AboutViewModel()
        {
            Version = ThisAssembly.AssemblyVersion;
            OpenBrowserCommand = new OpenBrowserCommand();
        }
    }
}
