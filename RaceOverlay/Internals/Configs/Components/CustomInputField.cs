using System.Windows.Controls;
using System.Windows.Media;

namespace RaceOverlay.Internals.Configs.Components;


public class CustomInputField: TextBox
{
    public CustomInputField(String text)
    {
        Text = text;
        Background = new SolidColorBrush(Color.FromArgb(255, 62, 62, 62));
        Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
    }
}