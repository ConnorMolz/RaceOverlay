using System.Diagnostics;
using System.Windows;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.LaptimeDelta;

public partial class LaptimeDelta : Overlay
{
    private iRacingData _data;
    private double _laptimeDelta = 0;
    
    //TODO: Add description
    public LaptimeDelta() : base("Lap Time Delta", "This Overlay...")
    {
        InitializeComponent();

        _setWindowSize(200, 65);
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }

    public override void _updateWindow()
    {
        //TODO: Implement Logic
        if (_laptimeDelta < 0)
        {
            //DeltaText.Text = ?;
            DeltaBarPositive.Width = 0;
            //DeltaBarNegative.Width = ?;
            return;
        }

        if (_laptimeDelta > 0)
        {
            //DeltaText.Text = ?;
            DeltaBarNegative.Width = 0;
            //DeltaBarPositive.Width = ?;
            return;
        }

        if (_laptimeDelta == 0)
        {
            //DeltaText.Text = ?;
            DeltaBarNegative.Width = 0;
            DeltaBarPositive.Width = 0;
            return;
        }
    }

    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        _laptimeDelta = _data.LocalDriver.BestLapDelta;
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