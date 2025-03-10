using System.Diagnostics;
using System.Windows;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.LaptimeDelta;

public partial class LaptimeDelta : Overlay
{
    //TODO: Add description
    public LaptimeDelta() : base("Lap Time Delta", "This Overlay...")
    {
        InitializeComponent();

        _setWindowSize(160, 65);
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        
        updateThread.IsBackground = true;
        updateThread.Start();
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
                if (IsVisible)
                {
                    _getData();
                    
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