using System.Windows.Controls;
using System.Windows.Media;

namespace RaceOverlay.Overlays.Leaderboard;

public class LeaderBoardRow: Grid
{
    
    public string DriverName { get; set; }
    public int Position { get; set; }
    public float LastLap { get; set; }
    public float BestLap { get; set; }
    public int IRating { get; set;}

    public LeaderBoardRow(string driverName, int position, float lastLap, float bestLap, int rating, string classColorCode)
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
        positionTextBlock.Background = _getClassColorBrush(classColorCode);
        
        positionTextBlock.SetValue(Grid.ColumnProperty, 0);
        positionTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(positionTextBlock);
        
        TextBlock driverNameTextBlock = new TextBlock();
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
        string iRatingText = (IRating / 1000).ToString("F1") + "k";
        iRatingTextBlock.Text = iRatingText;
        
        iRatingTextBlock.SetValue(Grid.ColumnProperty, 4);
        iRatingTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(iRatingTextBlock);
    }
    
    private SolidColorBrush _getClassColorBrush(string classColorCode)
    {
        if (!(new BrushConverter().ConvertFromString(classColorCode) is SolidColorBrush))
        {
            var brush = new SolidColorBrush();
            brush.Color = Colors.White;
            return brush;
        }

        return new BrushConverter().ConvertFromString(classColorCode) as SolidColorBrush;
    }
    
}