using Microsoft.UI.Xaml.Controls;

namespace Windows12.UI_Components.StartMenu
{
    public sealed partial class StartMenu : UserControl
    {
        public StartMenu()
        {
            this.InitializeComponent();
        }

        public void ExecuteLocalSearch(string query)
        {
            // Local NPU-powered search: indexes filesystem, settings, and Dev Workspace only.
            // No cloud or telemetry.
        }

        private void OnAppIconClicked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Launch via NUPS CorePC bridge with Time-Travel snapshot optional
        }
    }

    public sealed class AppTile : UserControl
    {
        public AppTile()
        {
            // Simple tile template for pinned apps
        }

        public string Icon { get; set; } = "";
        public string Name { get; set; } = "";
    }
}
