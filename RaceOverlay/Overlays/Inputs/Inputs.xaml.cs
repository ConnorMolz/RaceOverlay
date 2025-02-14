using System.Windows;
using System.Windows.Media;

namespace RaceOverlay.Overlays.Inputs;

public partial class Inputs : Window
{
    public Inputs()
    {
        InitializeComponent();
        DrawHardcodedData();
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
}