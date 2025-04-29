using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;
using RaceOverlay.Internals.Configs;

namespace RaceOverlay.Overlays.Inputs;

public partial class Inputs : Overlay
{
    private double _throttle;
    private double _brake;
    private double _clutch;
    private int _gear;
    private double _speed;
    private double _steering;

    private bool _showSteering;
    
    private iRacingData _data;
    
    public Inputs():base("Inputs", "Displays the current inputs, current gear and current speed of the car.")
    {
        InitializeComponent();
        _getConfig();
        _setWindowSize(_calcWitdh(), 60);
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }

    public override void _updateWindow()
    {
        try
        {
            ThrottleBar.Height = _throttle * 50;
            BrakeBar.Height = _brake * 50;
            ClutchBar.Height = _clutch * 50;
            GearText.Text = formatGear(_gear);
            SpeedText.Text = _speed.ToString("F0");
            SteeringRotation.Angle = _steering;

        }
        catch (Exception e)
        {
            //ignore
        }
        
    }

    protected override void _getConfig()
    {
        _showSteering = _getBoolConfig("ShowSteering");
    }

    public override Grid GetConfigs()
    {
        Grid grid = new Grid();
        
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        
        grid.RowDefinitions.Add(new RowDefinition());
        
        CheckBoxElement showSteering = new CheckBoxElement("Show Steering: ", _showSteering);
        showSteering.CheckBox.Checked += (sender, args) =>
        {
            _showSteering = true;
            _setBoolConfig("ShowSteering", true);
            _setWindowSize(_calcWitdh(), 60);
        };
        showSteering.CheckBox.Unchecked += (sender, args) =>
        {
            _showSteering = false;
            _setBoolConfig("ShowSteering", false);
            _setWindowSize(_calcWitdh(), 60);
        };
        
        Grid.SetRow(showSteering, 0);
        Grid.SetColumn(showSteering, 0);
        grid.Children.Add(showSteering);
        
        return grid;
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
            if (!_devMode)
            {
                InCar = _data.InCar;
            }
            else
            {
                InCar = true;
            }

            if (_showSteering)
            {
                _steering = _data.Inputs.Steering;
            }
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

    private string formatGear(int value)
    {
        return value switch
        {
            -1 => "R",
            0 => "N",
            _ => value.ToString()
        };
    }
    
    private int _calcWitdh()
    {
        if (_showSteering)
        {
            SteeringWheelGrid.Visibility = Visibility.Visible;
            return 200;
        }
        else
        {
            SteeringWheelGrid.Visibility = Visibility.Collapsed;
            return 140;
        }
    }
}