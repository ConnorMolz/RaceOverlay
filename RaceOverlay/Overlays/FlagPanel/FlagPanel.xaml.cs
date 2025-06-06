using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using RaceOverlay.Data;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.FlagPanel;

public partial class FlagPanel : Overlay
{

    private IrsdkFlags _flag;
    private bool _wasGreen;
    public FlagPanel(): base("Flag Panel", "This Overlay shows the current flag state")
    {
        InitializeComponent();
        _setWindowSize(100, 100);
        LoadEmbeddedImage(CheckeredFlag, "RaceOverlay.Overlays.FlagPanel.checkered_flag.png");
        LoadEmbeddedImage(DsqFlag, "RaceOverlay.Overlays.FlagPanel.dsq_flag.png");
        LoadEmbeddedImage(RepairFlag, "RaceOverlay.Overlays.FlagPanel.meetball_flag.png");
                LoadEmbeddedImage(DebrisFlag, "RaceOverlay.Overlays.FlagPanel.debris_flag.png");
        SetGreen();
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }

    public override void _updateWindow()
    {
        if (_flag.HasFlag(IrsdkFlags.Disqualify))
        {
            SetDsq();
            _wasGreen = false;
            return;
        }

        if (_flag.HasFlag(IrsdkFlags.Checkered))
        {
            SetCheckered();
            _wasGreen = false;
            return;
        }
        
        if (_flag.HasFlag(IrsdkFlags.Black) || _flag.HasFlag(IrsdkFlags.Furled))
        {
            SetBlack();
            _wasGreen = false;
            return;
        }

        if (_flag.HasFlag(IrsdkFlags.Debris))
        {
            _wasGreen = false;
            SetDebris();
            return;
        }

        if (_flag.HasFlag(IrsdkFlags.Repair))
        {
            _wasGreen = false;
            SetRepair();
            return;
        }

        if (_flag.HasFlag(IrsdkFlags.Red))
        {
            SetRed();
            _wasGreen = false;
            return;
        }
        
        if (_flag.HasFlag(IrsdkFlags.Blue))
        {
            SetBlue();
            _wasGreen = false;
            return;
        }

        if (_flag.HasFlag(IrsdkFlags.CautionWaving) || _flag.HasFlag(IrsdkFlags.Caution) || _flag.HasFlag(IrsdkFlags.Yellow) || _flag.HasFlag(IrsdkFlags.YellowWaving))
        {
            SetYellow();
            _wasGreen = false;
            return;
        }

        if (_flag.HasFlag(IrsdkFlags.White))
        {
            SetWhite();
            _wasGreen = false;
            return;
        }
        
        if (_flag.HasFlag(IrsdkFlags.Green))
        {
            SetGreen();
            return;
        }

        if (!(
                _flag.HasFlag(IrsdkFlags.Disqualify) || 
                _flag.HasFlag(IrsdkFlags.Black) || 
                _flag.HasFlag(IrsdkFlags.Furled) || 
                _flag.HasFlag(IrsdkFlags.Red) || 
                _flag.HasFlag(IrsdkFlags.Blue) ||
                _flag.HasFlag(IrsdkFlags.CautionWaving) || 
                _flag.HasFlag(IrsdkFlags.Caution) || 
                _flag.HasFlag(IrsdkFlags.Yellow) || 
                _flag.HasFlag(IrsdkFlags.YellowWaving) ||
                _flag.HasFlag(IrsdkFlags.White) ||
                _flag.HasFlag(IrsdkFlags.Checkered) ||
                _flag.HasFlag(IrsdkFlags.Debris) ||
                _flag.HasFlag(IrsdkFlags.Repair) ||
                _flag.HasFlag(IrsdkFlags.Green)
            ) )
        {
            FlagCanvas.Background = new BrushConverter().ConvertFromString("#FF393939") as SolidColorBrush;
            CheckeredFlag.Visibility = Visibility.Collapsed;
            DsqFlag.Visibility = Visibility.Collapsed;
            FlagCanvas.Visibility = Visibility.Visible;
            DebrisFlag.Visibility = Visibility.Collapsed;
            RepairFlag.Visibility = Visibility.Collapsed;
        }
    }

    public override void _getData()
    {
        if (!_devMode)
        {
            InCar = MainWindow.IRacingData.InCar;
        }
        else
        {
            InCar = true;
        }
        _flag = MainWindow.IRacingData.LocalDriver.CurrentIrsdkFlags;
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
        DebrisFlag.Visibility = Visibility.Collapsed;
        RepairFlag.Visibility = Visibility.Collapsed;
        FlagCanvas.Background = Brushes.Lime;
    }

    private void SetRed()
    {
        FlagCanvas.Background = Brushes.Red;
        CheckeredFlag.Visibility = Visibility.Collapsed;
        DsqFlag.Visibility = Visibility.Collapsed;
        FlagCanvas.Visibility = Visibility.Visible;
        DebrisFlag.Visibility = Visibility.Collapsed;
        RepairFlag.Visibility = Visibility.Collapsed;
    }
    
    private void SetYellow()
    {
        FlagCanvas.Background = Brushes.Yellow;
        CheckeredFlag.Visibility = Visibility.Collapsed;
        DsqFlag.Visibility = Visibility.Collapsed;
        FlagCanvas.Visibility = Visibility.Visible;
        DebrisFlag.Visibility = Visibility.Collapsed;
        RepairFlag.Visibility = Visibility.Collapsed;
    }
    
    private void SetBlue()
    {
        FlagCanvas.Background = Brushes.Blue;
        CheckeredFlag.Visibility = Visibility.Collapsed;
        DsqFlag.Visibility = Visibility.Collapsed;
        FlagCanvas.Visibility = Visibility.Visible;
        DebrisFlag.Visibility = Visibility.Collapsed;
        RepairFlag.Visibility = Visibility.Collapsed;
    }
    
    private void SetWhite()
    {
        FlagCanvas.Background = Brushes.White;
        CheckeredFlag.Visibility = Visibility.Collapsed;
        DsqFlag.Visibility = Visibility.Collapsed;
        FlagCanvas.Visibility = Visibility.Visible;
        DebrisFlag.Visibility = Visibility.Collapsed;
        RepairFlag.Visibility = Visibility.Collapsed;
    }
    
    private void SetBlack()
    {
        FlagCanvas.Background = Brushes.Black;
        CheckeredFlag.Visibility = Visibility.Collapsed;
        DsqFlag.Visibility = Visibility.Collapsed;
        FlagCanvas.Visibility = Visibility.Visible;
        DebrisFlag.Visibility = Visibility.Collapsed;
        RepairFlag.Visibility = Visibility.Collapsed;
    }
    
    private void SetCheckered()
    {
        CheckeredFlag.Visibility = Visibility.Visible;
        DsqFlag.Visibility = Visibility.Collapsed;
        FlagCanvas.Visibility = Visibility.Collapsed;
        DebrisFlag.Visibility = Visibility.Collapsed;
        RepairFlag.Visibility = Visibility.Collapsed;
    }

    private void SetDsq()
    {
        CheckeredFlag.Visibility = Visibility.Collapsed;
        DsqFlag.Visibility = Visibility.Visible;
        FlagCanvas.Visibility = Visibility.Collapsed;
        DebrisFlag.Visibility = Visibility.Collapsed;
        RepairFlag.Visibility = Visibility.Collapsed;
    }
    
    private void SetRepair()
    {
        CheckeredFlag.Visibility = Visibility.Collapsed;
        DsqFlag.Visibility = Visibility.Collapsed;
        FlagCanvas.Visibility = Visibility.Collapsed;
        DebrisFlag.Visibility = Visibility.Collapsed;
        RepairFlag.Visibility = Visibility.Visible;
    }
    
    private void SetDebris()
    {
        CheckeredFlag.Visibility = Visibility.Collapsed;
        DsqFlag.Visibility = Visibility.Collapsed;
        FlagCanvas.Visibility = Visibility.Collapsed;
        DebrisFlag.Visibility = Visibility.Visible;
        RepairFlag.Visibility = Visibility.Collapsed;
    }
}