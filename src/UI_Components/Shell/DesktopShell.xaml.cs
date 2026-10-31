using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Windows12.UI_Components.Shell;

public sealed partial class DesktopShell : Page
{
    public DesktopShell()
    {
        this.InitializeComponent();
        UpdateClock();
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
        timer.Tick += (_, _) => UpdateClock();
        timer.Start();
    }

    private void UpdateClock()
    {
        ClockText.Text = DateTime.Now.ToString("HH:mm");
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        StartMenu.Visibility = StartMenu.Visibility == Visibility.Visible
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private void Search_Click(object sender, RoutedEventArgs e)
    {
        StartMenu.Visibility = Visibility.Visible;
    }

    private void Explorer_Click(object sender, RoutedEventArgs e)
    {
        OpenApp("Explorer");
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        OpenApp("Settings");
    }

    private void DesktopIcon_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string tag)
            OpenApp(tag);
    }

    private void StartApp_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string tag)
        {
            StartMenu.Visibility = Visibility.Collapsed;
            OpenApp(tag);
        }
    }

    private void Power_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Exit();
    }

    private void OpenApp(string app)
    {
        StartMenu.Visibility = Visibility.Collapsed;
        ContentFrame.Visibility = Visibility.Visible;
        ContentFrame.Content = new TextBlock
        {
            Text = $"{app} \u2014 Windows 12",
            FontSize = 28,
            Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
    }
}
