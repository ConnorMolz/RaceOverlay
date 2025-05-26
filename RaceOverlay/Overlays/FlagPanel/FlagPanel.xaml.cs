using System.Windows;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.FlagPanel;

public partial class FlagPanel : Overlay
{
    public FlagPanel(): base("Flag Panel", "This Overlay shows the current flag state")
    {
        InitializeComponent();
    }

    public override void _updateWindow()
    {
        throw new NotImplementedException();
    }

    public override void _getData()
    {
        throw new NotImplementedException();
    }

    protected override void _scaleWindow(double scale)
    {
        throw new NotImplementedException();
    }
}