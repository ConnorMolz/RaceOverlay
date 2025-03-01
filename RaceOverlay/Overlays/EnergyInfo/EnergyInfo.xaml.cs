using System.Windows;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.EnergyInfo;

public partial class EnergyInfo : Overlay
{
    public EnergyInfo(): base("Engery Info", "Displays the current energy level of the battery. (Only available in GPT)")
    {
        InitializeComponent();
    }

    public override void _updateWindow()
    {
        
    }
    
    public override void _getData()
    {
        
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