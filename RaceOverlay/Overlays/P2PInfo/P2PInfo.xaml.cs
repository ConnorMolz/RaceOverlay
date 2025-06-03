using System.Diagnostics;
using System.Windows;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.P2PInfo;

public partial class P2PInfo : Overlay
{
    private float _p2pLeft;
    private bool _p2pOn;
    
    public P2PInfo(): base("P2P Info", "Displays the current P2P status and remaining time.")
    {
        InitializeComponent();
        _setWindowSize(200, 35);
        Thread updateThread = new Thread(UpdateThreadMethod);
        updateThread.IsBackground = true;
        updateThread.Start();
    }

    public override void _updateWindow()
    {
        if (_p2pLeft > 200)
        {
            P2PBar.Width = 200;
        }
        else
        {
            P2PBar.Width = _p2pLeft;
        }

        P2PText.Text = _p2pLeft.ToString("f0");
        if (_p2pOn)
        {
            P2PActive.Visibility = Visibility.Visible;
            P2PInactive.Visibility = Visibility.Collapsed;
        }
        {
            P2PActive.Visibility = Visibility.Collapsed;
            P2PInactive.Visibility = Visibility.Visible;
        }


    }

    public override void _getData()
    {
        if (_devMode)
        {
            InCar = true;
        }
        else
        {
            InCar = MainWindow.IRacingData.InCar;
        }

        _p2pLeft = MainWindow.IRacingData.LocalCarTelemetry.P2PLeft;
        _p2pOn = MainWindow.IRacingData.LocalCarTelemetry.P2POn;
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