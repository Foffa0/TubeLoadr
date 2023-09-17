using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace YoutubeDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Minimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void WindowStateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void MouseDown_RemoveFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = new TextBox();
            if (FocusManager.GetFocusedElement(this) != null)
            {
                if (FocusManager.GetFocusedElement(this).GetType() == textBox.GetType())
                {
                    Keyboard.ClearFocus();
                    //FocusManager.SetFocusedElement(this, textBox);
                }
            }
        }
    }
}
