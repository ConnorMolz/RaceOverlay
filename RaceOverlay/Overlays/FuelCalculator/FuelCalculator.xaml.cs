using System.Windows;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.FuelCalculator;

public partial class FuelCalculator : Overlay
{
    public FuelCalculator() : base("","")
    {
        InitializeComponent();
        _setWindowSize(300, 30);
        
        _getConfig();
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }

    public override void _updateWindow()
    {
        throw new NotImplementedException();
    }

    public override void _getData()
    {
        throw new NotImplementedException();
    }

    public override void UpdateThreadMethod()
    {
        throw new NotImplementedException();
    }

    protected override void _scaleWindow(double scale)
    {
        throw new NotImplementedException();
    }
}