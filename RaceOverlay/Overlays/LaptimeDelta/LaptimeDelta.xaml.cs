using System.Diagnostics;
using System.Windows;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.LaptimeDelta;

public partial class LaptimeDelta : Overlay
{
    private iRacingData _data;
    private double _laptimeDelta = 0;
    
    public LaptimeDelta() : base("Lap Time Delta", "This Overlay shows the current delta to the best lap time which is currently set by the local car.")
    {
        InitializeComponent();

        _setWindowSize(300, 30);
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }

    public override void _updateWindow()
    {
        if (_laptimeDelta < 0)
        {
            var barWidth = (15 * (_laptimeDelta * -1)) * _scale;
            if(barWidth > 150) barWidth = 150;
            
            DeltaText.Text = _laptimeDelta.ToString("F3");
            DeltaBarPositive.Width = 0;
            DeltaBarNegative.Width = barWidth;
            return;
        }

        if (_laptimeDelta > 0)
        {
            var barWidth = (15 * _laptimeDelta) * _scale;
            if(barWidth > 150) barWidth = 150;
            DeltaText.Text = "+" + _laptimeDelta.ToString("F3");
            DeltaBarNegative.Width = 0;
            DeltaBarPositive.Width = barWidth;
            return;
        }

        if (_laptimeDelta == 0)
        {
            DeltaText.Text = _laptimeDelta.ToString("F3");
            DeltaBarNegative.Width = 0;
            DeltaBarPositive.Width = 0;
            return;
        }
    }

    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        _laptimeDelta = _data.LocalDriver.BestLapDelta;
        if (!_devMode)
        {
            _inCar = _data.InCar;
        }
        else
        {
            _inCar = true;
        }
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