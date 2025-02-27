using System.Reflection;
using System.Windows;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.Inputs;

public partial class Inputs : Overlay
{
    private String _overlayName = "Inputs";
    private String _overlayDescription = "Displays the current inputs, current gear and current speed of the car.";
    
    private double _throttle;
    private double _brake;
    private double _clutch;
    private int _gear;
    private double _speed;

    private iRacingData _data;
    
    public Inputs()
    {
        InitializeComponent();
        
        Thread updateThread = new Thread(UpdateThreadMethod);

        OverlayName = _overlayName;
        OverlayDescription = _overlayDescription;
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }

    public override void _updateWindow()
    {
        try
        {
            ThrottleBar.Height = _throttle * 100;
            BrakeBar.Height = _brake * 100;
            ClutchBar.Height = _clutch * 100;
            GearText.Text = _gear.ToString();
            SpeedText.Text = _speed.ToString("F0");
        }
        catch (Exception e)
        {
            //ignore
        }
        
    }
   

    public override void _getData()
    {
        try
        {
            _data = MainWindow.IRacingData;
            _throttle = _data.Inputs.Throttle;
            _brake = _data.Inputs.Brake;
            _clutch = 1 - _data.Inputs.Clutch;
            _gear = _data.LocalCarTelemetry.Gear;
            _speed = _data.LocalCarTelemetry.Speed;
        }
        catch (TargetInvocationException e)
        {
            Console.WriteLine("IRacing data not available yet.");
            Console.WriteLine(e);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error getting iRacing data: " + e.Message);
        }
        
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