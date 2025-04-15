using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.TrackCircle;

public partial class TrackCircle : Overlay
{
    private iRacingData _data;
    public TrackCircle() : base("Track Circle", "This Overlay shows the track as a circle with car markers on the track")
    {
        InitializeComponent();
    }

    public override void _updateWindow()
    {
        CircleCanvas.Children.Clear();
    
        // Add Ellipse for the track
        Ellipse trackEllipse = new Ellipse();
        trackEllipse.Width = 400;
        trackEllipse.Height = 400;
        trackEllipse.StrokeThickness = 10;
        trackEllipse.Stroke = Brushes.White;
        CircleCanvas.Children.Add(trackEllipse);
        
        
        var drivers = _data.Drivers;
        for (int i = 0; i < drivers.Length; i++)
        {
            // Position based on LapDistance
            double angle = drivers[i].LapDistance * 2 * Math.PI;
            double radius = 185; 
            double centerX = 200; 
            double centerY = 200;
        
            double posX = centerX + radius * Math.Sin(angle) - 7.5; 
            double posY = centerY - radius * Math.Cos(angle) - 7.5; 
        
            // Ellipse for every car
            Ellipse car = new Ellipse();
            car.Width = 15;
            car.Height = 15;
            car.Fill = GetColor(drivers[i].ClassColorCode);
            car.Stroke = Brushes.White;
            car.StrokeThickness = 1;
        
            Canvas.SetLeft(car, posX);
            Canvas.SetTop(car, posY);
            CircleCanvas.Children.Add(car);
        
           
            TextBlock positionText = new TextBlock();
            positionText.Text = drivers[i].ClassPosition.ToString();
            positionText.Foreground = Brushes.White;
            positionText.FontSize = 10;
            positionText.FontWeight = FontWeights.Bold;
        
            // Center Text
            positionText.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            double textWidth = positionText.DesiredSize.Width;
            double textHeight = positionText.DesiredSize.Height;
        
            Canvas.SetLeft(positionText, posX + (15 - textWidth) / 2);
            Canvas.SetTop(positionText, posY + (15 - textHeight) / 2);
            CircleCanvas.Children.Add(positionText);
            
        }
    }

    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        if (_devMode)
        {
            _inCar = true;
            return;
        }
        else
        {
            _inCar = _data.InCar;
        }
        
    }

    protected override void _scaleWindow(double scale)
    {
        try
        {
            ContentScaleTransform.ScaleX = scale;
            ContentScaleTransform.ScaleY = scale;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }
    
    private Brush GetColor(string colorCode)
    {
        if (colorCode == "#ffffff" || colorCode == "#000000" || colorCode == "#undefined")
        {
            var brush = new SolidColorBrush();
            brush.Color = Colors.Navy;
            return brush;
        }
        if (!(new BrushConverter().ConvertFromString(colorCode) is SolidColorBrush))
        {
            var brush = new SolidColorBrush();
            brush.Color = Colors.Navy;
            return brush;
        }

        return new BrushConverter().ConvertFromString(colorCode) as SolidColorBrush;
    }
    
    
}