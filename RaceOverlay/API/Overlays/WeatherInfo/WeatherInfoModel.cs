namespace RaceOverlay.API.Overlays.WeatherInfo;

public class WeatherInfoModel
{
    public float AirTemp { get; set; }
    public float TrackTemp { get; set; }
    public float Precipitation { get; set; }
    public bool IsWet { get; set; }
    
    
    public WeatherInfoModel()
    {
        AirTemp = MainWindow.IRacingData.WeatherData.AirTemp;
        TrackTemp = MainWindow.IRacingData.WeatherData.TrackTemp;
        Precipitation = MainWindow.IRacingData.WeatherData.Precipitation;
        IsWet = MainWindow.IRacingData.WeatherData.WeatherDeclaredWet;
    }
}