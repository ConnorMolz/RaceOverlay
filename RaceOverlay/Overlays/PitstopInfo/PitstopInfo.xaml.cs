using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.PitstopInfo;

public partial class PitstopInfo : Overlay
{
    private iRacingData _data;

    private bool _enablePitHelper;
    private float _reqRepairTime;
    private float _optRepairTime;
    
    //TODO: Add description
    public PitstopInfo() : base("Pitstop Info", "TODO")
    {
        InitializeComponent();
        
        _setWindowSize(210, 65);
        
        _loadConfig();
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        updateThread.IsBackground = true;
        updateThread.Start();
    }
    

    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        _reqRepairTime = _data.Pitstop.RequiredRepairTimeLeft;
        _optRepairTime = _data.Pitstop.OptionalRepairTimeLeft;

    }
    
    public override void _updateWindow()
    {
        ReqRepairTimeText.Text = _reqRepairTime.ToString();
        OptRepairTimeText.Text = _optRepairTime.ToString();
    }

    public override void UpdateThreadMethod()
    {
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
                Thread.Sleep(2000); // 1 update per 2 seconds
            }
        }
    }
    

    //TODO: If pithelper is implemented remove comment form add child operations
    public override Grid GetConfigs()
    {
        Grid grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition());
        
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        
        TextBlock pitHelpertextBlock = new TextBlock();
        pitHelpertextBlock.Text = "Activate Pitstop Helper: ";
        
        Grid.SetRow(pitHelpertextBlock, 0);
        Grid.SetColumn(pitHelpertextBlock, 0);
        //grid.Children.Add(pitHelpertextBlock);
        
        CheckBox pitHelperCheckBox = new CheckBox();
        pitHelperCheckBox.IsChecked = _enablePitHelper;
        
        Grid.SetRow(pitHelperCheckBox, 0);
        Grid.SetColumn(pitHelperCheckBox, 1);
        pitHelperCheckBox.Checked += (sender, args) =>
        {
            _enablePitHelper = true;
            _setBoolConfig("_enablePitHelper", _enablePitHelper);
        };
        pitHelperCheckBox.Unchecked += (sender, args) =>
        {
            _enablePitHelper = false;
            _setBoolConfig("_enablePitHelper", _enablePitHelper);
        };
        //grid.Children.Add(pitHelperCheckBox);

        return grid;
    }

    protected override void _loadConfig()
    {
       _enablePitHelper = _getBoolConfig("_enablePitHelper");
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