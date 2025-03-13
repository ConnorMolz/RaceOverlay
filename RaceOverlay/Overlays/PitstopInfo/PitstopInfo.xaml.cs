using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.PitstopInfo;

public partial class PitstopInfo : Overlay
{
    private iRacingData _data;

    private bool _enableTyreInfo;
    private float _reqRepairTime;
    private float _optRepairTime;
    private float _needFuel;
    
    private float _tyreGreenUntil;
    private float _tyreYellowUntil;
    
    private Tyre _tyreFL;
    private Tyre _tyreFR;
    private Tyre _tyreRL;
    private Tyre _tyreRR;
    
    private bool _intoPit = false;
    
    //TODO: Add description
    public PitstopInfo() : base("Pitstop Info", "TODO")
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
        _reqRepairTime = _data.Pitstop.RequiredRepairTimeLeft;
        _optRepairTime = _data.Pitstop.OptionalRepairTimeLeft;
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
        
    }
    
    public override void _updateWindow()
    {
        ReqRepairTimeText.Text = _reqRepairTime.ToString();
        OptRepairTimeText.Text = _optRepairTime.ToString();
        FuelNeededText.Text = _needFuel.ToString();
        
        // Tyre Infos
        //FL
        FLTyreWearTxtL.Text = _tyreFL.WearLeft.ToString("F0") + "%";
        FLTyreWearTxtC.Text = _tyreFL.WearCenter.ToString("F0") + "%";
        FlTyreWearTxtR.Text = _tyreFL.WearRight.ToString("F0") + "%";
        
        //FR
        FRTyreWearTxtL.Text = _tyreFR.WearLeft.ToString("F0") + "%";
        FRTyreWearTxtC.Text = _tyreFR.WearCenter.ToString("F0") + "%";
        FRTyreWearTxtR.Text = _tyreFR.WearRight.ToString("F0") + "%";
        
        //RL
        RLTyreWearTxtL.Text = _tyreRL.WearLeft.ToString("F0") + "%";
        RLTyreWearTxtC.Text = _tyreRL.WearCenter.ToString("F0") + "%";
        RLTyreWearTxtR.Text = _tyreRL.WearRight.ToString("F0") + "%";
        
        //RR
        RRTyreWearTxtL.Text = _tyreRR.WearLeft.ToString("F0") + "%";
        RRTyreWearTxtC.Text = _tyreRR.WearCenter.ToString("F0") + "%";
        RRTyreWearTxtR.Text = _tyreRR.WearRight.ToString("F0") + "%";
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
                    if (_intoPit)
                    {
                        Dispatcher.Invoke(() => { _updateWindow(); });
                    }
                }
                
                // Add a small delay to prevent high CPU usage
                Thread.Sleep(2000); // 1 update per 2 seconds
            }
        }
    }
    

    //TODO: If pithelper is implemented remove comment form add child operations
    public override Grid GetConfigs()
    {
        Grid grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        
        
        // Tyre Info toggle
        TextBlock tyreInfotextBlock = new TextBlock();
        tyreInfotextBlock.Text = "Activate Tyre info after stop: ";
        
        Grid.SetRow(tyreInfotextBlock, 0);
        Grid.SetColumn(tyreInfotextBlock, 0);
        grid.Children.Add(tyreInfotextBlock);
        
        CheckBox tyreInfoCheckBox = new CheckBox();
        tyreInfoCheckBox.IsChecked = _enableTyreInfo;
        
        Grid.SetRow(tyreInfoCheckBox, 0);
        Grid.SetColumn(tyreInfoCheckBox, 1);
        tyreInfoCheckBox.Checked += (sender, args) =>
        {
            _enableTyreInfo = true;
            _setBoolConfig("_enableTyreInfo", _enableTyreInfo);
        };
        tyreInfoCheckBox.Unchecked += (sender, args) =>
        {
            _enableTyreInfo = false;
            _setBoolConfig("_enableTyreInfo", _enableTyreInfo);
        };
        grid.Children.Add(tyreInfoCheckBox);
        
        // Color picker of tyre color by wear
        TextBlock showTyreGreenUntil = new TextBlock();
        showTyreGreenUntil.Text = "Show Tyre Green until wear of: ";
        Grid.SetColumn(showTyreGreenUntil, 0);
        Grid.SetRow(showTyreGreenUntil, 1);
        grid.Children.Add(showTyreGreenUntil);
        
        TextBox tyreGreenUntil = new TextBox();
        tyreGreenUntil.Text = _tyreGreenUntil.ToString();
        Grid.SetColumn(tyreGreenUntil, 1);
        Grid.SetRow(tyreGreenUntil, 1);
        grid.Children.Add(tyreGreenUntil);
        //TODO: Add event for updating _tyreGreenUntil
        
        // Color picker of tyre color by wear
        TextBlock showTyreYellowUntil = new TextBlock();
        showTyreYellowUntil.Text = "Show Tyre Green until wear of: ";
        Grid.SetColumn(showTyreYellowUntil, 0);
        Grid.SetRow(showTyreYellowUntil, 2);
        grid.Children.Add(showTyreYellowUntil);
        
        TextBox tyreYellowUntil = new TextBox();
        tyreYellowUntil.Text = _tyreYellowUntil.ToString();
        Grid.SetColumn(tyreYellowUntil, 1);
        Grid.SetRow(tyreYellowUntil, 2);
        grid.Children.Add(tyreYellowUntil);
        //TODO: Add event for updating _tyreGreenUntil

        return grid;
    }

    protected override void _loadConfig()
    {
       _enableTyreInfo = _getBoolConfig("_enableTyreInfo");
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
    
    private int calcRemainingLaps(float time, float lapTime)
    {
        return 0;
    }

    private float claculateFuelNeeded(float laps, float currentFuel)
    {
        return 0;
    }
}