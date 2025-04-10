using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;
using RaceOverlay.Internals.Configs;

namespace RaceOverlay.Overlays.PitstopInfo;

public partial class PitstopInfo : Overlay
{
    private iRacingData _data;

    private bool _enableTyreInfo;
    private TimeSpan _reqRepairTime;
    private TimeSpan _optRepairTime;
    private float _needFuel;
    private float _avgFuelPerLap;

    private float _marginLaps;
    private float _tyreGreenUntil;
    private float _tyreYellowUntil;
    
    private Tyre _tyreFL;
    private Tyre _tyreFR;
    private Tyre _tyreRL;
    private Tyre _tyreRR;
    
    private SolidColorBrush green = new SolidColorBrush(Color.FromRgb(0, 255, 0));
    private SolidColorBrush yellow = new SolidColorBrush(Color.FromRgb(255, 255, 0));
    private SolidColorBrush red = new SolidColorBrush(Color.FromRgb(255, 0, 0));
    
    private bool _intoPit = false;
    
    public PitstopInfo() : base("Pitstop Info", "This Overlay shows the current Repair Times and Tyre Status if car is standing in pitbox.\nThis Overlay is only show if the car is on pitroad")
    {
        InitializeComponent();
        
        _setWindowSize(240, 370);
        
        _loadConfig();
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        updateThread.IsBackground = true;
        updateThread.Start();
    }
    

    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        // Repair Times and Fuel Calc
        _reqRepairTime = TimeSpan.FromSeconds(_data.Pitstop.RequiredRepairTimeLeft);
        _optRepairTime = TimeSpan.FromSeconds(_data.Pitstop.OptionalRepairTimeLeft);
        //_avgFuelPerLap = 
        // TODO: FUEL CALC
        /*_needFuel = claculateFuelNeeded(
            calcRemainingLaps(0, 0),
            _data.LocalCarTelemetry.FuelLevel
        );*/
        _needFuel = 0;
        
        // Tyre Infos
        _tyreFL = _data.LocalCarTelemetry.FrontLeftTyre;
        _tyreFR = _data.LocalCarTelemetry.FrontRightTyre;
        _tyreRL = _data.LocalCarTelemetry.RearLeftTyre;
        _tyreRR = _data.LocalCarTelemetry.RearRightTyre;
        _intoPit = _data.Pitstop.InPit;
        
        // Get in car info
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
        Show();
        ReqRepairTimeText.Text = $"{_reqRepairTime:hh\\:mm\\:ss}";
        OptRepairTimeText.Text = $"{_optRepairTime:hh\\:mm\\:ss}";
        //FuelNeededText.Text = _needFuel.ToString();
        
        // Tyre Infos
        //FL
        FLTyreWearTxtL.Text = _tyreFL.WearLeft.ToString("F0") + "%";
        FLTyreWearTxtC.Text = _tyreFL.WearCenter.ToString("F0") + "%";
        FlTyreWearTxtR.Text = _tyreFL.WearRight.ToString("F0") + "%";
        FLTyreWearL.Fill = getTryeColor(_tyreFL.WearLeft);
        FLTyreWearC.Fill = getTryeColor(_tyreFL.WearCenter);
        FLTyreWearR.Fill = getTryeColor(_tyreFL.WearRight);
        
        //FR
        FRTyreWearTxtL.Text = _tyreFR.WearLeft.ToString("F0") + "%";
        FRTyreWearTxtC.Text = _tyreFR.WearCenter.ToString("F0") + "%";
        FRTyreWearTxtR.Text = _tyreFR.WearRight.ToString("F0") + "%";
        FRTyreWearL.Fill = getTryeColor(_tyreFR.WearLeft);
        FRTyreWearC.Fill = getTryeColor(_tyreFR.WearCenter);
        FRTyreWearR.Fill = getTryeColor(_tyreFR.WearRight);
        
        //RL
        RLTyreWearTxtL.Text = _tyreRL.WearLeft.ToString("F0") + "%";
        RLTyreWearTxtC.Text = _tyreRL.WearCenter.ToString("F0") + "%";
        RLTyreWearTxtR.Text = _tyreRL.WearRight.ToString("F0") + "%";
        RLTyreWearL.Fill = getTryeColor(_tyreRL.WearLeft);
        RLTyreWearC.Fill = getTryeColor(_tyreRL.WearCenter);
        RLTyreWearR.Fill = getTryeColor(_tyreRL.WearRight);
        
        //RR
        RRTyreWearTxtL.Text = _tyreRR.WearLeft.ToString("F0") + "%";
        RRTyreWearTxtC.Text = _tyreRR.WearCenter.ToString("F0") + "%";
        RRTyreWearTxtR.Text = _tyreRR.WearRight.ToString("F0") + "%";
        RRTyreWearL.Fill = getTryeColor(_tyreRR.WearLeft);
        RRTyreWearC.Fill = getTryeColor(_tyreRR.WearCenter);
        RRTyreWearR.Fill = getTryeColor(_tyreRR.WearRight);
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
                    if (_intoPit)
                    {
                        Dispatcher.Invoke(() => { _updateWindow(); });
                    }
                    else
                    {
                        Dispatcher.Invoke(() => { Hide(); });
                    }
                }
                
                // Add a small delay to prevent high CPU usage
                Thread.Sleep(2000); // 1 update per 2 seconds
            }
        }
    }
    
    
    public override Grid GetConfigs()
    {
        Grid grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        
        // Color picker of tyre color by wear
        InputElement showTyreGreenUntilELement = new InputElement("Show Tyre Green until wear of: ", _tyreGreenUntil.ToString("F0"));
        
        Grid.SetRow(showTyreGreenUntilELement, 1);
        grid.Children.Add(showTyreGreenUntilELement);
        
        void ParseUntilGreenInput(object sender, TextChangedEventArgs e)
        {
            if (float.TryParse(showTyreGreenUntilELement.InputField.Text, out float maxValue))
            {
                _tyreGreenUntil = maxValue;
                _setFloatConfig("_tyreGreenUntil", _tyreYellowUntil);
            }
        }

        showTyreGreenUntilELement.InputField.TextChanged += ParseUntilGreenInput;
        
        // Color picker of tyre color by wear
        InputElement showTyreYellowUntilELement = new InputElement("Show Tyre Yellow until wear of: ", _tyreYellowUntil.ToString("F0"));
        
        void ParseUntilYellowInput(object sender, TextChangedEventArgs e)
        {
            if (float.TryParse(showTyreYellowUntilELement.Label.Text, out float maxValue))
            {
                _tyreYellowUntil = maxValue;
                _setFloatConfig("_tyreYellowUntil", _tyreYellowUntil);
            }
        }
        grid.Children.Add(showTyreYellowUntilELement);

        showTyreYellowUntilELement.InputField.TextChanged += ParseUntilYellowInput;

        return grid;
    }

    protected override void _loadConfig()
    {
       _enableTyreInfo = _getBoolConfig("_enableTyreInfo");
       _marginLaps = _getFloatConfig("_marginLaps");
       _tyreGreenUntil = _getFloatConfig("_tyreGreenUntil"); 
       _tyreYellowUntil = _getFloatConfig("_tyreYellowUntil");

       if (_tyreGreenUntil == 0)
       {
           _tyreGreenUntil =75.0f;
           _setFloatConfig("_tyreGreenUntil", _tyreGreenUntil);
       }
       
       if (_tyreYellowUntil == 0)
       {
           _tyreYellowUntil = 50.0f;
           _setFloatConfig("_tyreYellowUntil", _tyreYellowUntil);
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
    
    private float calcRemainingLaps(float time, float lapTime)
    {
        return time/lapTime + 1;
    }

    private float claculateFuelNeeded(float laps, float currentFuel)
    {
        return ((laps * _avgFuelPerLap) - currentFuel) + (_marginLaps * _avgFuelPerLap);
    }

    private SolidColorBrush getTryeColor(float tyreWear)
    {
        if(tyreWear >= _tyreGreenUntil)
        {
            return green;
        }
        else if (tyreWear >= _tyreYellowUntil)
        {
            return yellow;
        }
        else
        {
            return red;
        }
    }

    
}