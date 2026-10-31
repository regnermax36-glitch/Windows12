using Microsoft.UI.Xaml;
using Windows12.UI_Components.Installer;

namespace Windows12.UI_Components
{
    public partial class App : Application
    {
        public static Window? MainWindow { get; private set; }

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            MainWindow = new Window();
            MainWindow.Content = new InstallerPage();
            MainWindow.Title = "Windows 12 Installer";
            MainWindow.AppWindow.Resize(new Windows.Graphics.SizeInt32(900, 600));
            MainWindow.Activate();
        }
    }
}
