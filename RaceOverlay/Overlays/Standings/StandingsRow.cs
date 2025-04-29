using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RaceOverlay.Overlays.Leaderboard;

public class StandingsRow: Grid
{
    
    public string DriverName { get; set; }
    public string Position { get; set; }
    public float LastLap { get; set; }
    public float BestLap { get; set; }
    public int IRating { get; set;}
    
    TextBlock driverNameTextBlock { get; set; }

    public StandingsRow(string driverName, int carNr, int position, float lastLap, float bestLap, int rating, string classColorCode, int gapToLeader, string interval, string safetyRating)
    {
        Height = 20;
        
        DriverName = driverName;
        Position = position.ToString();
        LastLap = lastLap;
        BestLap = bestLap;
        IRating = rating;
        
        if (Position == "0")
        {
            Position = "-";
        }
        
        RowDefinitions.Add(new RowDefinition());
        
        ColumnDefinition positionColumnDefinition = new ColumnDefinition();
        positionColumnDefinition.Width = GridLength.Auto;
        
        ColumnDefinition carNrColumnDefinition = new ColumnDefinition();
        carNrColumnDefinition.Width = GridLength.Auto;
        
        ColumnDefinition driverNameColumnDefinition = new ColumnDefinition();
        driverNameColumnDefinition.Width = GridLength.Auto;
        
        ColumnDefinition lastLapColumnDefinition = new ColumnDefinition();
        lastLapColumnDefinition.Width = GridLength.Auto;
        
        ColumnDefinition bestLapColumnDefinition = new ColumnDefinition();
        bestLapColumnDefinition.Width = GridLength.Auto;
        
        ColumnDefinition safetyColumnDefinition = new ColumnDefinition();
        safetyColumnDefinition.Width = GridLength.Auto;
        
        ColumnDefinition ratingColumnDefinition = new ColumnDefinition();
        ratingColumnDefinition.Width = GridLength.Auto;
        
        ColumnDefinition gapColumnDefinition = new ColumnDefinition();
        gapColumnDefinition.Width = GridLength.Auto;
        
        ColumnDefinition intervalColumnDefinition = new ColumnDefinition();
        intervalColumnDefinition.Width = GridLength.Auto;
        
        ColumnDefinitions.Add(positionColumnDefinition);
        ColumnDefinitions.Add(carNrColumnDefinition);
        ColumnDefinitions.Add(driverNameColumnDefinition);
        ColumnDefinitions.Add(lastLapColumnDefinition);
        ColumnDefinitions.Add(bestLapColumnDefinition);
        ColumnDefinitions.Add(safetyColumnDefinition);
        ColumnDefinitions.Add(ratingColumnDefinition);
        ColumnDefinitions.Add(gapColumnDefinition);
        ColumnDefinitions.Add(intervalColumnDefinition);
        
        
        TextBlock positionTextBlock = new TextBlock();
        positionTextBlock.Text = Position.ToString();
        positionTextBlock.Background = _getClassColorBrush(classColorCode.Replace("0x", "#"));
        positionTextBlock.TextAlignment = TextAlignment.Center;
        positionTextBlock.Width = 15;
        positionTextBlock.Height = 30;
        
        positionTextBlock.SetValue(Grid.ColumnProperty, 0);
        positionTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(positionTextBlock);
        
        
        TextBlock carNrTextBlock = new TextBlock();
        carNrTextBlock.Text = "#" + carNr;
        carNrTextBlock.TextAlignment = TextAlignment.Center;
        carNrTextBlock.Width = 26;
        carNrTextBlock.Height = 30;
        
        carNrTextBlock.SetValue(Grid.ColumnProperty, 1);
        carNrTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(carNrTextBlock);
        
        
        driverNameTextBlock = new TextBlock();
        driverNameTextBlock.Text = DriverName;
        driverNameTextBlock.TextAlignment = TextAlignment.Left;
        driverNameTextBlock.Margin = new Thickness(5, 0, 0, 0);
        driverNameTextBlock.Width = 195;
        
        driverNameTextBlock.SetValue(Grid.ColumnProperty, 2);
        driverNameTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(driverNameTextBlock);
        
        
        TextBlock lastLapTextBlock = new TextBlock();
        
        TimeSpan lastLapTimeSpan = TimeSpan.FromSeconds(LastLap);
        string lastLapText = $"{lastLapTimeSpan:mm\\:ss\\:fff}";
        
        lastLapTextBlock.Text = lastLapText;
        lastLapTextBlock.TextAlignment = TextAlignment.Center;
        lastLapTextBlock.Width = 80;
        lastLapTextBlock.Height = 30;
        
        lastLapTextBlock.SetValue(Grid.ColumnProperty, 3);
        lastLapTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(lastLapTextBlock);
        
        
        TextBlock bestLapTextBlock = new TextBlock();

        TimeSpan bestLapTimeSpan = TimeSpan.FromSeconds(bestLap);
        string bestLapTimeText = $"{bestLapTimeSpan:mm\\:ss\\:fff}";
        
        bestLapTextBlock.Text = bestLapTimeText;
        bestLapTextBlock.TextAlignment = TextAlignment.Center;
        bestLapTextBlock.Width = 80;
        bestLapTextBlock.Height = 30;
        
        bestLapTextBlock.SetValue(Grid.ColumnProperty, 4);
        bestLapTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(bestLapTextBlock);
        
        
        TextBlock safetyTextBlock = new TextBlock();
        safetyTextBlock.Text = safetyRating;
        safetyTextBlock.TextAlignment = TextAlignment.Center;
        safetyTextBlock.Width = 35;
        safetyTextBlock.Height = 30;
        safetyTextBlock.Background = _getLicenseColorBrush(safetyRating);
        safetyTextBlock.Foreground = _getLicenseTextColor(safetyRating);
        
        safetyTextBlock.SetValue(Grid.ColumnProperty, 5);
        safetyTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(safetyTextBlock);
        
        
        TextBlock iRatingTextBlock = new TextBlock();
        string iRatingText = ((float)IRating / 1000).ToString("F1") + "k";
        iRatingTextBlock.Text = iRatingText;
        iRatingTextBlock.TextAlignment = TextAlignment.Center;
        iRatingTextBlock.Width = 30;
        iRatingTextBlock.Height = 30;
        
        iRatingTextBlock.SetValue(Grid.ColumnProperty, 6);
        iRatingTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(iRatingTextBlock);
        
        TextBlock gapTextBlock = new TextBlock();
        gapTextBlock.Text = TimeSpan.FromMilliseconds(gapToLeader).ToString(@"ss\.f");
        gapTextBlock.TextAlignment = TextAlignment.Center;
        gapTextBlock.Width = 45;
        
        gapTextBlock.SetValue(Grid.ColumnProperty, 6);
        gapTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(gapTextBlock);
  
        TextBlock intervalTextBlock = new TextBlock();
        intervalTextBlock.Text = interval;
        intervalTextBlock.TextAlignment = TextAlignment.Center;
        intervalTextBlock.Width = 45;
        
        intervalTextBlock.SetValue(Grid.ColumnProperty, 7);
        intervalTextBlock.SetValue(Grid.RowProperty, 0);
        Children.Add(intervalTextBlock);
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