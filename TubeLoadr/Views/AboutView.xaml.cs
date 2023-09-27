using System.Diagnostics;
using System.Windows.Navigation;

namespace TubeLoadr.Views
{

    public partial class AboutView
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ProcessStartInfo ProcessStartInfo = new ProcessStartInfo
            {
                FileName = e.Uri.ToString(),
                UseShellExecute = true
            };
            Process.Start(ProcessStartInfo);
        }
    }
}
