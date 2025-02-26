using System.Reflection;
using System.Windows;
using RaceOverlay.Data.Models;

namespace RaceOverlay.Overlays.Inputs;

public partial class Inputs : Window
{
    
    private double _throttle;
    private double _brake;
    private double _clutch;
    private int _gear;
    private double _speed;

    private iRacingData _data;
    
    public Inputs()
    {
        InitializeComponent();
        _getData();
        _updateWindow();
        
        Thread updateThread = new Thread(() =>
        {
            while (IsVisible)
            {
                _getData();
                _updateWindow();
            }
        }); 
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }

    private void _updateWindow()
    {
        ThrottleBar.Height = _throttle * 100;
        BrakeBar.Height = _brake * 100;
        ClutchBar.Height = _clutch * 100;
        GearText.Text = _gear.ToString();
        SpeedText.Text = _speed.ToString("F0");
    }
   

    private void _getData()
    {
        try
        {
            _throttle = _data.Inputs.Throttle;
            _brake = _data.Inputs.Brake;
            _clutch = _data.Inputs.Clutch;
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
}