using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RaceOverlay.Overlays.Leaderboard;

public class StandingsRow: Grid
{
    
    public string DriverName { get; set; }
    public int Position { get; set; }
    public float LastLap { get; set; }
    public float BestLap { get; set; }
    public int IRating { get; set;}
    
    TextBlock driverNameTextBlock { get; set; }

    public StandingsRow(string driverName, int position, float lastLap, float bestLap, int rating, string classColorCode)
    {
        DriverName = driverName;
        Position = position;
        LastLap = lastLap;
        BestLap = bestLap;
        IRating = rating;
        
        RowDefinitions.Add(new RowDefinition());
        
        ColumnDefinitions.Add(new ColumnDefinition());
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
        
        driverNameTextBlock = new TextBlock();
        driverNameTextBlock.Text = DriverName;
        
        driverNameTextBlock.SetValue(Grid.ColumnProperty, 1);
        driverNameTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(driverNameTextBlock);
        
        TextBlock lastLapTextBlock = new TextBlock();
        TimeSpan lastLapTimeSpan = TimeSpan.FromSeconds(LastLap);
        lastLapTextBlock.Text = $"{lastLapTimeSpan:mm\\:ss\\:fff}";
        
        lastLapTextBlock.SetValue(Grid.ColumnProperty, 2);
        lastLapTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(lastLapTextBlock);
        
        TextBlock bestLapTextBlock = new TextBlock();
        TimeSpan bestLapTimeSpan = TimeSpan.FromSeconds(bestLap);
        lastLapTextBlock.Text = $"{bestLapTimeSpan:mm\\:ss\\:fff}";
        
        bestLapTextBlock.SetValue(Grid.ColumnProperty, 3);
        bestLapTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(bestLapTextBlock);
        
        TextBlock iRatingTextBlock = new TextBlock();
        string iRatingText = ((float)IRating / 1000).ToString("F1") + "k";
        iRatingTextBlock.Text = iRatingText;
        
        iRatingTextBlock.SetValue(Grid.ColumnProperty, 4);
        iRatingTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(iRatingTextBlock);
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