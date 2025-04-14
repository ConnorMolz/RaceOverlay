using System.Diagnostics;
using System.Windows;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.EnergyInfo;

public partial class EnergyInfo : Overlay
{
    private float _energyLevelPct;  
    
    private iRacingData _data;
    
    
    public EnergyInfo(): base("Engery Info", "Displays the current energy level of the battery. (Only available in GPT)")
    {
        InitializeComponent();
        
        _setWindowSize(200, 35);
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }

    public override void _updateWindow()
    {
        EnergyPctText.Text = _energyLevelPct.ToString("F1") + "%";
        EnergyBar.Width = _energyLevelPct * 2;
    }
    
    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        _energyLevelPct = _data.LocalCarTelemetry.EngeryLevelPct * 1;
        if (!_devMode)
        {
            InCar = _data.InCar;
        }
        else
        {
            InCar = true;
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