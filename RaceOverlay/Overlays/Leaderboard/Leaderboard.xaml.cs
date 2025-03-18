using System.Diagnostics;
using System.Windows;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.Leaderboard;

public partial class Leaderboard : Overlay
{
    // TODO: Add Description
    public Leaderboard(): base("Leaderboard", "TODO")
    {
        InitializeComponent();
        _setWindowSize(200, 35);
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }

    public override void _updateWindow()
    {
        
    }

    public override void _getData()
    {
        
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