using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Windows12.UI_Components.Installer;

public sealed partial class InstallerPage : Page
{
    public InstallerPage()
    {
        this.InitializeComponent();
        if (!InstallerEngine.IsAdministrator())
        {
            StatusText.Text = "This installer must be run as Administrator.";
            InstallButton.IsEnabled = false;
        }
    }

    private async void InstallButton_Click(object sender, RoutedEventArgs e)
    {
        InstallButton.IsEnabled = false;
        ProgressBar.IsIndeterminate = true;
        StatusText.Text = "Installing...";

        await System.Threading.Tasks.Task.Run(() =>
        {
            InstallerEngine.Install(msg =>
            {
                DispatcherQueue.TryEnqueue(() => StatusText.Text = msg);
            });
        });

        ProgressBar.IsIndeterminate = false;
        ProgressBar.Value = 100;
        StatusText.Text = "Installation complete. Restart recommended.";
        InstallButton.Content = "Done";
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Exit();
    }
}
