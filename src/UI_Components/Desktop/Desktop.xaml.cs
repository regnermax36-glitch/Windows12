using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Windows12.UI_Components.Desktop
{
    public sealed partial class Desktop : Page
    {
        public Desktop()
        {
            this.InitializeComponent();
        }
    }

    public sealed class DesktopIcon : UserControl
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(string), typeof(DesktopIcon), new PropertyMetadata("&#xE8B7;"));
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(nameof(Label), typeof(string), typeof(DesktopIcon), new PropertyMetadata("Icon"));

        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public DesktopIcon()
        {
            var panel = new StackPanel { Spacing = 6, Width = 88 };
            var icon = new FontIcon { FontSize = 42, HorizontalAlignment = HorizontalAlignment.Center, Foreground = new SolidColorBrush(Microsoft.UI.Colors.White) };
            var text = new TextBlock { HorizontalAlignment = HorizontalAlignment.Center, Foreground = new SolidColorBrush(Microsoft.UI.Colors.White), FontSize = 12, TextWrapping = TextWrapping.Wrap, TextAlignment = TextAlignment.Center };
            panel.Children.Add(icon);
            panel.Children.Add(text);
            this.Content = panel;
            // Bindings applied at runtime or via x:Bind in full implementation
        }
    }
}
