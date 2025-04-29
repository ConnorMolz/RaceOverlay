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

    public RelativeRow(string driverName, int position, float distance, int carNr, string classColorCode, string? safety="")
    {
        Height = 20;
        
        DriverName = driverName;
        Position = position;
        Distance = distance;
        CarNr = carNr;
        
        RowDefinitions.Add(new RowDefinition());
        
        ColumnDefinition positionColumn = new ColumnDefinition();
        positionColumn.Width = GridLength.Auto;
        
        ColumnDefinition carNrColumn = new ColumnDefinition();
        carNrColumn.Width = GridLength.Auto;
        
        ColumnDefinition driverNameColumn = new ColumnDefinition();
        driverNameColumn.Width = GridLength.Auto;
        
        ColumnDefinition distanceColumn = new ColumnDefinition();
        distanceColumn.Width = GridLength.Auto;
        
        ColumnDefinition safetyColumn = new ColumnDefinition();
        safetyColumn.Width = GridLength.Auto;
        
        ColumnDefinitions.Add(positionColumn);
        ColumnDefinitions.Add(carNrColumn);
        ColumnDefinitions.Add(driverNameColumn);
        ColumnDefinitions.Add(safetyColumn);
        ColumnDefinitions.Add(distanceColumn);
        
        TextBlock positionTextBlock = new TextBlock();
        positionTextBlock.Text = Position + ".";
        positionTextBlock.Background = _getClassColorBrush(classColorCode.Replace("0x", "#"));
        positionTextBlock.Height = 20;
        positionTextBlock.Width = 15;
        positionTextBlock.Margin = new Thickness(0, 0, 5, 0);
        positionTextBlock.VerticalAlignment = VerticalAlignment.Center;
        
        positionTextBlock.SetValue(Grid.ColumnProperty, 0);
        positionTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(positionTextBlock);
        
        
        TextBlock carNrTextBlock = new TextBlock();
        carNrTextBlock.Text = "#" + CarNr;
        carNrTextBlock.Width = 26;
        carNrTextBlock.Height = 20;
        carNrTextBlock.Margin = new Thickness(0, 0, 5, 0);
        carNrTextBlock.VerticalAlignment = VerticalAlignment.Center;

        carNrTextBlock.SetValue(Grid.ColumnProperty, 1);
        carNrTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(carNrTextBlock);
        
        
        driverNameTextBlock = new TextBlock();
        driverNameTextBlock.Text = DriverName;
        driverNameTextBlock.Width = 190;
        driverNameTextBlock.Height = 20;
        driverNameTextBlock.Margin = new Thickness(0, 0, 5, 0);
        driverNameTextBlock.VerticalAlignment = VerticalAlignment.Center;
        
        driverNameTextBlock.SetValue(Grid.ColumnProperty, 2);
        driverNameTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(driverNameTextBlock);
        
        TextBlock safetyTextBlock = new TextBlock();
        safetyTextBlock.Text = safety;
        safetyTextBlock.Margin = new Thickness(0, 0, 5, 0);
        safetyTextBlock.TextAlignment = TextAlignment.Left;
        safetyTextBlock.Width = 35;
        safetyTextBlock.Height = 20;
        safetyTextBlock.Background = _getLicenseColorBrush(safety);
        safetyTextBlock.Foreground = _getLicenseTextColor(safety);
        safetyTextBlock.VerticalAlignment = VerticalAlignment.Center;
        
        safetyTextBlock.SetValue(Grid.ColumnProperty, 3);
        safetyTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(safetyTextBlock);
        
        TextBlock distanceTextBlock = new TextBlock();
        distanceTextBlock.Text = TimeSpan.FromMilliseconds(distance).ToString(@"ss\.f");
        distanceTextBlock.TextAlignment = TextAlignment.Right;
        distanceTextBlock.VerticalAlignment = VerticalAlignment.Center;
        distanceTextBlock.Margin = new Thickness(0, 0, 5, 0);
        distanceTextBlock.Width = 45;
        distanceTextBlock.Height = 20;
        
        distanceTextBlock.SetValue(Grid.ColumnProperty, 4);
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

    private SolidColorBrush _getLicenseColorBrush(string safetyRating)
    {
        switch (safetyRating[0])
        {
            case 'D':
                return new SolidColorBrush(Colors.Orange);
            case 'C':
                return new SolidColorBrush(Colors.Yellow);
            case 'B':
                return new SolidColorBrush(Colors.Green);
            case 'A':
                return new SolidColorBrush(Colors.Blue);
            case 'P':
                return new SolidColorBrush(Colors.Black);
            default:
                return new SolidColorBrush(Colors.Red);
        }
    }
    
    private SolidColorBrush _getLicenseTextColor(string safetyRating)
    {
        switch (safetyRating[0])
        {
            case 'D':
                return new SolidColorBrush(Colors.Black);
            case 'C':
                return new SolidColorBrush(Colors.Black);
            case 'B':
                return new SolidColorBrush(Colors.Black);
            case 'A':
                return new SolidColorBrush(Colors.White);
            case 'P':
                return new SolidColorBrush(Colors.White);
            default:
                return new SolidColorBrush(Colors.White);
        }
    }
}