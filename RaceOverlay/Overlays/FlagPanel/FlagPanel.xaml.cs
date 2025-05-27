using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.FlagPanel;

public partial class FlagPanel : Overlay
{
    public FlagPanel(): base("Flag Panel", "This Overlay shows the current flag state")
    {
        InitializeComponent();
        _setWindowSize(100, 100);
        LoadEmbeddedImage(CheckeredFlag, "RaceOverlay.Overlays.FlagPanel.checkered_flag.png");
        LoadEmbeddedImage(DsqFlag, "RaceOverlay.Overlays.FlagPanel.dsq_flag.png");
        SetGreen();
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

    private void SetGreen()
    {
        CheckeredFlag.Visibility = Visibility.Collapsed;
        DsqFlag.Visibility = Visibility.Collapsed;
        FlagCanvas.Visibility = Visibility.Visible;
        FlagCanvas.Background = Brushes.Lime;
        
        Thread setOutThread = new Thread(() =>
        {
            Thread.Sleep(5000);
            Dispatcher.Invoke(() =>
            {
                FlagCanvas.Background = new BrushConverter().ConvertFromString("#FF393939") as SolidColorBrush;
                CheckeredFlag.Visibility = Visibility.Collapsed;
                DsqFlag.Visibility = Visibility.Collapsed;
                FlagCanvas.Visibility = Visibility.Visible;
            });
        });
        setOutThread.Start();
    }

    private void SetRed()
    {
        FlagCanvas.Background = Brushes.Red;
        CheckeredFlag.Visibility = Visibility.Collapsed;
        DsqFlag.Visibility = Visibility.Collapsed;
        FlagCanvas.Visibility = Visibility.Visible;
    }
    
    private void SetYellow()
    {
        FlagCanvas.Background = Brushes.Yellow;
        CheckeredFlag.Visibility = Visibility.Collapsed;
        DsqFlag.Visibility = Visibility.Collapsed;
        FlagCanvas.Visibility = Visibility.Visible;
    }
    
    private void SetBlue()
    {
        FlagCanvas.Background = Brushes.Blue;
        CheckeredFlag.Visibility = Visibility.Collapsed;
        DsqFlag.Visibility = Visibility.Collapsed;
        FlagCanvas.Visibility = Visibility.Visible;
    }
    
    private void SetWhite()
    {
        FlagCanvas.Background = Brushes.White;
        CheckeredFlag.Visibility = Visibility.Collapsed;
        DsqFlag.Visibility = Visibility.Collapsed;
        FlagCanvas.Visibility = Visibility.Visible;
    }
    
    private void SetBlack()
    {
        FlagCanvas.Background = Brushes.Black;
        CheckeredFlag.Visibility = Visibility.Collapsed;
        DsqFlag.Visibility = Visibility.Collapsed;
        FlagCanvas.Visibility = Visibility.Visible;
    }
    
    private void SetCheckered()
    {
        CheckeredFlag.Visibility = Visibility.Visible;
        DsqFlag.Visibility = Visibility.Collapsed;
        FlagCanvas.Visibility = Visibility.Collapsed;
    }

    private void SetDsq()
    {
        CheckeredFlag.Visibility = Visibility.Collapsed;
        DsqFlag.Visibility = Visibility.Visible;
        FlagCanvas.Visibility = Visibility.Collapsed;
    }
}