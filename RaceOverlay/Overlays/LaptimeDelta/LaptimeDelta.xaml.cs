using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.LaptimeDelta;

public partial class LaptimeDelta : Overlay
{
    private iRacingData _data;
    private double _laptimeDelta = 0;

    private bool _useBestLap;
    
    public LaptimeDelta() : base("Lap Time Delta", "This Overlay shows the current delta to the best lap time which is currently set by the local car.")
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
        if (_laptimeDelta < 0)
        {
            var barWidth = (15 * (_laptimeDelta * -1)) * _scale;
            if(barWidth > 150) barWidth = 150;
            
            DeltaText.Text = _laptimeDelta.ToString("F3");
            DeltaBarPositive.Width = 0;
            DeltaBarNegative.Width = barWidth;
            return;
        }

        if (_laptimeDelta > 0)
        {
            var barWidth = (15 * _laptimeDelta) * _scale;
            if(barWidth > 150) barWidth = 150;
            DeltaText.Text = "+" + _laptimeDelta.ToString("F3");
            DeltaBarNegative.Width = 0;
            DeltaBarPositive.Width = barWidth;
            return;
        }

        if (_laptimeDelta == 0)
        {
            DeltaText.Text = _laptimeDelta.ToString("F3");
            DeltaBarNegative.Width = 0;
            DeltaBarPositive.Width = 0;
            return;
        }
    }

    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        _laptimeDelta = _data.LocalDriver.BestLapDelta;
        if (!_devMode)
        {
            InCar = _data.InCar;
        }
        else
        {
            InCar = true;
        }
        if (_useBestLap)
        {
            _laptimeDelta = _data.LocalDriver.BestLapDelta;
            return;
        }

        _laptimeDelta = _data.LocalDriver.LastLapDelta;
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

    protected override void _getConfig()
    {
        _useBestLap = _getBoolConfig("_useBestLap");
    }

    public override Grid GetConfigs()
    {
        Grid grid = new Grid();
        
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        
        TextBlock bestLapLabel = new TextBlock();
        bestLapLabel.Text = "Best Lap Delta";
        
        RadioButton bestLapRadio = new RadioButton();
        bestLapRadio.IsChecked = _useBestLap;
        
        bestLapRadio.Checked += (sender, args) =>
        {
            _useBestLap = true;
            _setBoolConfig("_useBestLap", _useBestLap);
        };
        bestLapRadio.Unchecked += (sender, args) =>
        {
            _useBestLap = false;
            _setBoolConfig("_useBestLap", _useBestLap);
        };
        
        
        TextBlock lastLapLabel = new TextBlock();
        lastLapLabel.Text = "Last Lap Delta";
        
        RadioButton lastLapRadio = new RadioButton();
        lastLapRadio.IsChecked = !_useBestLap;
        
        lastLapRadio.Checked += (sender, args) =>
        {
            _useBestLap = false;
            _setBoolConfig("_useBestLap", _useBestLap);
        };
        lastLapRadio.Unchecked += (sender, args) =>
        {
            _useBestLap = true;
            _setBoolConfig("_useBestLap", _useBestLap);
        };
        
        Grid.SetColumn(bestLapLabel, 0);
        Grid.SetRow(bestLapLabel, 0);
        grid.Children.Add(bestLapLabel);
        
        Grid.SetColumn(bestLapRadio, 1);
        Grid.SetRow(bestLapRadio, 0);
        grid.Children.Add(bestLapRadio);
        
        Grid.SetColumn(lastLapLabel, 0);
        Grid.SetRow(lastLapLabel, 1);
        grid.Children.Add(lastLapLabel);
        
        Grid.SetColumn(lastLapRadio, 1);
        Grid.SetRow(lastLapRadio, 1);
        grid.Children.Add(lastLapRadio);
        
        return grid;

    }
}