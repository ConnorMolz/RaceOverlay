using System.Windows;
using System.Windows.Media;
using HerboldRacing;
using RaceOverlay.Data.Models;

namespace RaceOverlay.Overlays.Inputs;

public partial class Inputs : Window
{
    
    private double _throttle;
    private double _brake;
    private double _clutch;
    private int _gear;
    private double _speed;

    private iRacingData data;
    
    public Inputs()
    {
        InitializeComponent();
        DrawHardcodedData();
        data = MainWindow.IRacingData;
        _getData();
    }

    private void DrawHardcodedData()
    {
        // Sample data points for green line
        List<Point> greenPoints = new List<Point>
        {
            new Point(0, 80),
            new Point(50, 20),
            new Point(100, 20),
            new Point(150, 80),
            new Point(200, 80),
            new Point(250, 20),
            new Point(300, 40)
        };

        // Sample data points for red line
        List<Point> redPoints = new List<Point>
        {
            new Point(0, 80),
            new Point(50, 80),
            new Point(100, 20),
            new Point(150, 80),
            new Point(200, 80),
            new Point(250, 80),
            new Point(300, 80)
        };

        // Scale points to fit canvas
        ScalePoints(greenPoints);
        ScalePoints(redPoints);

        // Set points to polylines
        GreenLine.Points = new PointCollection(greenPoints);
        RedLine.Points = new PointCollection(redPoints);
    }

    private void ScalePoints(List<Point> points)
    {
        // Scale Y values to fit canvas (inverted because WPF coordinates go top-down)
        for (int i = 0; i < points.Count; i++)
        {
            points[i] = new Point(
                points[i].X,
                100 - points[i].Y
            );
        }
    }

    private void _getData()
    {
        _throttle = data.Inputs.Throttle;
        _brake = data.Inputs.Brake;
        _clutch = data.Inputs.Clutch;
        _gear = data.LocalCarTelemetry.Gear;
        _speed = data.LocalCarTelemetry.Speed;
    }
}