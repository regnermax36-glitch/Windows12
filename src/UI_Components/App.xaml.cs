using Microsoft.UI.Xaml;
using Windows12.UI_Components.Installer;

namespace Windows12.UI_Components
{
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var window = new Window();
            window.Content = new InstallerPage();
            window.Title = "Windows 12 Installer";
            window.Activate();
        }
    }
}
