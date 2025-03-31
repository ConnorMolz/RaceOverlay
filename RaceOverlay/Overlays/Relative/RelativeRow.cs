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
        DriverName = driverName;
        Position = position;
        Distance = distance;
        CarNr = carNr;
        
        RowDefinitions.Add(new RowDefinition());
        
        ColumnDefinitions.Add(new ColumnDefinition());
        ColumnDefinitions.Add(new ColumnDefinition());
        ColumnDefinitions.Add(new ColumnDefinition());
        ColumnDefinitions.Add(new ColumnDefinition());
        
        TextBlock positionTextBlock = new TextBlock();
        positionTextBlock.Text = Position.ToString();
        positionTextBlock.Background = _getClassColorBrush(classColorCode.Replace("0x", "#"));
        
        positionTextBlock.SetValue(Grid.ColumnProperty, 0);
        positionTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(positionTextBlock);
        
        
        TextBlock carNrTextBlock = new TextBlock();
        carNrTextBlock.Text = CarNr.ToString();

        carNrTextBlock.SetValue(Grid.ColumnProperty, 1);
        carNrTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(carNrTextBlock);
        
        
        driverNameTextBlock = new TextBlock();
        driverNameTextBlock.Text = DriverName;
        
        driverNameTextBlock.SetValue(Grid.ColumnProperty, 2);
        driverNameTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(driverNameTextBlock);
        
        
        TextBlock distanceTextBlock = new TextBlock();
        distanceTextBlock.Text = TimeSpan.FromSeconds(distance).ToString(@"ss\.fff");
        
        distanceTextBlock.SetValue(Grid.ColumnProperty, 3);
        distanceTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(distanceTextBlock);
    }
    
    private SolidColorBrush _getClassColorBrush(string classColorCode)
    {
        if (classColorCode == "#FFFFFF" || classColorCode == "#000000")
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