using System.Windows.Controls;
using System.Windows.Media;

namespace RaceOverlay.Internals.Configs;

public class CustomLabel: TextBlock
{
    public CustomLabel(String text)
    {
        Text = text;
        Height = 30;
        Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30));
        Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
    }
}