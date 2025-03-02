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
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }

    public override void _updateWindow()
    {
        EnergyPctText.Text = _energyLevelPct.ToString("F1") + "%";
        EnergyBar.Width = _energyLevelPct * 200;
    }
    
    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        _energyLevelPct = _data.LocalCarTelemetry.EngeryLevelPct * 100;
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