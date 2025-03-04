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
    
    public WeatherInfo(): base("Weather Info", "Displays the current weather conditions and a forecast for the next 15 and 30 Minutes.")
    {
        InitializeComponent();
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }

    public override void _getData()
    {
        base._getData();
        _data = MainWindow.IRacingData;
        _airTemp = _data.WeatherData.AirTemp;
        _trackTemp = _data.WeatherData.TrackTemp;
        _precipitation = _data.WeatherData.Precipitation;
        _isWet = _data.WeatherData.WeatherDeclaredWet;
    }
    
    public override void _updateWindow()
    {
        AirTempText.Text = _airTemp.ToString("F1");
        TrackTempText.Text = _trackTemp.ToString("F1");
        PrecipitationText.Text = _precipitation.ToString("F") + "%";
        if (_isWet)
        {
            IsWetText.Text = "WET";
            IsWetBorder.Background = new BrushConverter().ConvertFromString("#0000FF") as SolidColorBrush;
        }
        else
        {
            IsWetText.Text = "DRY";
            IsWetBorder.Background = new BrushConverter().ConvertFromString("#00FF00") as SolidColorBrush;
        }
        base._updateWindow();
    }

    public override void UpdateThreadMethod()
    {
        base.UpdateThreadMethod();
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
}