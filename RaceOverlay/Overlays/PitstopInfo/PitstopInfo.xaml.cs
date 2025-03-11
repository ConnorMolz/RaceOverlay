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
    
    //TODO: Add description
    public PitstopInfo() : base("Pitstop Info", "TODO")
    {
        InitializeComponent();
        
        _setWindowSize(150, 130);
        
        _loadConfig();
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        updateThread.IsBackground = true;
        updateThread.Start();
    }
    

    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        
    }
    
    public override void _updateWindow()
    {
        
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
    

    public override Grid GetConfigs()
    {
        Grid grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        

        return grid;
    }

    protected override void _loadConfig()
    {
       
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