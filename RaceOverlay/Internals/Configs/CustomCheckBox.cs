using System.Windows.Controls;
using System.Windows.Media;

namespace RaceOverlay.Internals.Configs;

public class CustomCheckBox: CheckBox
{
    public CustomCheckBox(bool isChecked)
    {
        IsChecked = isChecked;
        Width = 300;
        Height = 30;
        Background = new SolidColorBrush(Color.FromArgb(255, 122, 122, 122));
        Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        
    }
}