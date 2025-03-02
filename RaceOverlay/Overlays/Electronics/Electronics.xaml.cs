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
    
    
    public Electronics(): base("Electronics", "An Overlay for displaying in car electronics like TC, ABS, etc.")
    {
        InitializeComponent();
        
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
        base.UpdateThreadMethod();
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
}