using System.Diagnostics;
using System.Windows;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.Electronics;

public partial class Electronics : Overlay
{

    private float _abs;
    private float _tc1;
    private float _tc2;
    private float _brakeBias;

    private iRacingData _data;
    
    public Electronics(): base("Electronics", "An Overlay for displaying the in car adjustments of ABS, TC1, TC2 and Brake Bias(BB).")
    {
        InitializeComponent();

        _setWindowSize(160, 65);
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }
    
    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        _abs = _data.LocalCarTelemetry.Abs;
        _tc1 = _data.LocalCarTelemetry.Tc1;
        _tc2 = _data.LocalCarTelemetry.Tc2;
        _brakeBias = _data.LocalCarTelemetry.BrakeBias;
        if (!_devMode)
        {
            InCar = _data.InCar;
        }
        else
        {
            InCar = true;
        }
    }

    public override void _updateWindow()
    {
        absValue.Text = _abs.ToString();
        tc1Value.Text = _tc1.ToString();
        tc2Value.Text = _tc2.ToString();
        bbValue.Text = _brakeBias.ToString("F2");
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