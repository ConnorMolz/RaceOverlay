using System.Windows;
using System.Windows.Media;
using HerboldRacing;

namespace RaceOverlay.Overlays.Inputs;

public partial class Inputs : Window
{
    
    private double _throttle;
    private double _brake;
    private double _clutch;
    private int _gear;
    private double _speed;
    
    IRSDKSharper IrsdkSharper = null!;
    
    public Inputs()
    {
        InitializeComponent();
        IrsdkSharper = MainWindow.IrsdkSharper;
        _getData();
    }

   

    private void _getData()
    {
        Console.WriteLine(IrsdkSharper.Data.TelemetryDataProperties.Capacity);
    }
}