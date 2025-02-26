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
        _gear = 
    }
}