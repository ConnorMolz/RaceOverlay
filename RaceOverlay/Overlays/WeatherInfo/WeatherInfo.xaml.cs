using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;
using RaceOverlay.Internals.Configs;

namespace RaceOverlay.Overlays.WeatherInfo;

public partial class WeatherInfo : Overlay
{
    private iRacingData _data;
    private float _airTemp;
    private float _trackTemp;
    private float _precipitation;
    private bool _isWet;

    private string _backgroundColor = "#1E1E1E";
    private string _wetColor = "#0000FF";
    private string _dryColor = "#00FF00";
    
    private bool _isOn = true;

    private bool _blinkingIsActiv = true;
    
    public WeatherInfo(): base("Weather Info", "Displays the current temperature, precipitation and if the track declared to be wet from race control.")
    {
        InitializeComponent();
        
        _setWindowSize(150, 130);
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        updateThread.IsBackground = true;
        updateThread.Start();
        
        Thread blinkAnimationThread = new Thread(BlinkAnimationMethod);
        blinkAnimationThread.IsBackground = true;
        blinkAnimationThread.Start();
        
        _loadConfig();
    }

    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        _airTemp = _data.WeatherData.AirTemp;
        _trackTemp = _data.WeatherData.TrackTemp;
        _precipitation = _data.WeatherData.Precipitation;
        _isWet = _data.WeatherData.WeatherDeclaredWet;
        if (!_devMode)
        {
            InCar = _data.InCar;
        }
        else
        {
            InCar = true;
        }
    }
    
    public override void _updateWindow()
    {
        AirTempText.Text = _airTemp.ToString("F1") + "C°";
        TrackTempText.Text = _trackTemp.ToString("F1") + "C°";
        PrecipitationText.Text = _precipitation.ToString("F") + "%";
        if (_isWet)
        {
            IsWetText.Text = "WET";
        }
        else
        {
            IsWetText.Text = "DRY";
        }

        if (!_blinkingIsActiv)
        {
            if (_isWet)
            {
                IsWetBorder.Background = new BrushConverter().ConvertFromString(_wetColor) as SolidColorBrush;
            }
            else
            {
                IsWetBorder.Background = new BrushConverter().ConvertFromString(_dryColor) as SolidColorBrush;
            }
        }
    }

    public override void UpdateThreadMethod()
    {
        {
            while (true)
            {
                _getData();
                if (IsVisible)
                {
                    
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

    private void BlinkAnimationMethod()
    {
        while (true)
        {
            if (_blinkingIsActiv)
            {
                if (IsVisible)
                {
                    _getData();

                    // Use Dispatcher to update UI from background thread
                    Dispatcher.Invoke(() => { BlinkAnimation(); });
                }

                // Add a small delay to prevent high CPU usage
                Thread.Sleep(500); // 1 update per 2 seconds
            }
        }
    }

    private void BlinkAnimation()
    {
        if (_isOn)
        {
            IsWetBorder.Background = new BrushConverter().ConvertFromString(_backgroundColor) as SolidColorBrush;
            _isOn = false;
        }
        else
        {
            if (_isWet)
            {
                IsWetBorder.Background = new BrushConverter().ConvertFromString(_wetColor) as SolidColorBrush;
                _isOn = true;
            }
            else
            {
                IsWetBorder.Background = new BrushConverter().ConvertFromString(_dryColor) as SolidColorBrush;
                _isOn = true;
            }
        }
    }

    public override Grid GetConfigs()
    {
        Grid grid = new Grid();
        
        grid.RowDefinitions.Add(new RowDefinition());
        
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        
        CheckBoxElement blinkingCheckBoxElement = new CheckBoxElement("Toggle Blinking: ", _blinkingIsActiv);
        
        blinkingCheckBoxElement.CheckBox.Checked += (sender, args) =>
        {
            _blinkingIsActiv = true;
            _setBoolConfig("_blinkingIsActiv", true);
        };
        blinkingCheckBoxElement.CheckBox.Unchecked += (sender, args) =>
        {
            _blinkingIsActiv = false;
            _setBoolConfig("_blinkingIsActiv", false);
        };
        Grid.SetRow(blinkingCheckBoxElement, 0);
        Grid.SetColumn(blinkingCheckBoxElement, 0);
        
        grid.Children.Add(blinkingCheckBoxElement);

        return grid;
    }

    protected override void _loadConfig()
    {
        _blinkingIsActiv = _getBoolConfig("_blinkingIsActiv");
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