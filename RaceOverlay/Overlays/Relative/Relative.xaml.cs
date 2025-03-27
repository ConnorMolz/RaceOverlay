using System.Diagnostics;
using System.Windows;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.Relative;

public partial class Relative : Overlay
{
    //TODO: Add Description
    public Relative() : base("Relative", "")
    {
        InitializeComponent();
    }

    public override void _updateWindow()
    {
        throw new NotImplementedException();
    }

    public override void _getData()
    {
        throw new NotImplementedException();
    }

    public override void UpdateThreadMethod()
    {
        {
            while (true)
            {
                _getData();
                if (IsVisible)
                {
                    
                    // Use Dispatcher to update UI from background thread
                    Dispatcher.Invoke(() =>
                    {
                        _updateWindow();
                    });
                }
                
                // Add a small delay to prevent high CPU usage
                Thread.Sleep(16); // ~60 updates per second
            }
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

}