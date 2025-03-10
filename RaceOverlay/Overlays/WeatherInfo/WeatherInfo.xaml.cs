using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;

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
    }

    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        _airTemp = _data.WeatherData.AirTemp;
        _trackTemp = _data.WeatherData.TrackTemp;
        _precipitation = _data.WeatherData.Precipitation;
        _isWet = _data.WeatherData.WeatherDeclaredWet;
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

    private void BlinkAnimationMethod()
    {
        while (true)
        {
            if (IsVisible)
            {
                _getData();
                    
                // Use Dispatcher to update UI from background thread
                Dispatcher.Invoke(() =>
                {
                    BlinkAnimation();
                });
            }
                
            // Add a small delay to prevent high CPU usage
            Thread.Sleep(500); // 1 update per 2 seconds
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
    
    protected override void _scaleWindow(double scale)
    {
        try
        {
            ContentScaleTransform.ScaleX = scale;
            ContentScaleTransform.ScaleY = scale;
        }
        catch (Exception e)
        {
            this._scale = scale;
            _setDoubleConfig("_scale", scale);
            Debug.WriteLine(e);
        }
    }

    
}