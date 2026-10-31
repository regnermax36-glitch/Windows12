using Microsoft.UI.Xaml;
using Windows12.UI_Components.Installer;

namespace Windows12.UI_Components
{
    public partial class App : Application
    {
        private Window? _window;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = new Window();
            _window.Content = new InstallerPage();
            _window.Title = "Windows 12 Installer";

            // Ensure the window is visible and sized
            _window.AppWindow.Resize(new Windows.Graphics.SizeInt32(900, 600));
            _window.Activate();
        }
    }
}
