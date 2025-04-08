using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RaceOverlay.Overlays.Relative;

public class RelativeRow: Grid
{
    public string DriverName { get; set; }
    public int Position { get; set; }
    public float CarNr { get; set; }
    public float Distance { get; set; }
    
    private TextBlock driverNameTextBlock { get; set; }

    public RelativeRow(string driverName, int position, float distance, int carNr, string classColorCode)
    {
        Height = 30;
        
        DriverName = driverName;
        Position = position;
        Distance = distance;
        CarNr = carNr;
        
        RowDefinitions.Add(new RowDefinition());
        
        ColumnDefinition col1 = new ColumnDefinition();
        col1.Width = GridLength.Auto;
        
        ColumnDefinition col2 = new ColumnDefinition();
        col2.Width = GridLength.Auto;
        
        ColumnDefinition col3 = new ColumnDefinition();
        col3.Width = GridLength.Auto;
        
        ColumnDefinition col4 = new ColumnDefinition();
        col4.Width = GridLength.Auto;
        
        ColumnDefinitions.Add(col1);
        ColumnDefinitions.Add(col2);
        ColumnDefinitions.Add(col3);
        ColumnDefinitions.Add(col4);
        
        TextBlock positionTextBlock = new TextBlock();
        positionTextBlock.Text = Position + ".";
        positionTextBlock.Background = _getClassColorBrush(classColorCode.Replace("0x", "#"));
        positionTextBlock.Width = 15;
        positionTextBlock.Margin = new Thickness(0, 0, 5, 0);
        
        positionTextBlock.SetValue(Grid.ColumnProperty, 0);
        positionTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(positionTextBlock);
        
        
        TextBlock carNrTextBlock = new TextBlock();
        carNrTextBlock.Text = "#" + CarNr;
        carNrTextBlock.Width = 15 + CarNr.ToString().Length * 9;
        carNrTextBlock.Margin = new Thickness(0, 0, 5, 0);

        carNrTextBlock.SetValue(Grid.ColumnProperty, 1);
        carNrTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(carNrTextBlock);
        
        
        driverNameTextBlock = new TextBlock();
        driverNameTextBlock.Text = DriverName;
        driverNameTextBlock.Width = 15 + DriverName.ToString().Length * 9;
        driverNameTextBlock.Margin = new Thickness(0, 0, 5, 0);
        
        driverNameTextBlock.SetValue(Grid.ColumnProperty, 2);
        driverNameTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(driverNameTextBlock);
        
        
        TextBlock distanceTextBlock = new TextBlock();
        distanceTextBlock.Text = TimeSpan.FromMilliseconds(distance).ToString(@"ss\.f");
        distanceTextBlock.TextAlignment = TextAlignment.Right;
        distanceTextBlock.Margin = new Thickness(0, 0, 5, 0);
        
        distanceTextBlock.SetValue(Grid.ColumnProperty, 3);
        distanceTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(distanceTextBlock);
    }
    
    private SolidColorBrush _getClassColorBrush(string classColorCode)
    {
        if (classColorCode == "#ffffff" || classColorCode == "#000000" || classColorCode == "#undefined")
        {
            var brush = new SolidColorBrush();
            brush.Color = Colors.Navy;
            return brush;
        }
        if (!(new BrushConverter().ConvertFromString(classColorCode) is SolidColorBrush))
        {
            var brush = new SolidColorBrush();
            brush.Color = Colors.Navy;
            return brush;
        }

        return new BrushConverter().ConvertFromString(classColorCode) as SolidColorBrush;
    }
    
    public void SetToPlayerRow()
    {
        driverNameTextBlock.FontWeight = FontWeights.Bold;
        driverNameTextBlock.Foreground = Brushes.Gold;
    }
}