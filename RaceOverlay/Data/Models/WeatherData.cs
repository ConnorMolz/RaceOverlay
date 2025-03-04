namespace RaceOverlay.Data.Models;

public class WeatherData
{
    public float AirTemp { get; set; }
    public float TrackTemp { get; set; }
    public float RelativeHumidity { get; set; }
    public float FogLevel { get; set; }
    public float TrackTempCrew { get; set; }
    public float Skies { get; set; }
    public float TrackWetness { get; set; }
    public float Precipitation { get; set; }
    public float WindDir { get; set; }
    public float WindVel { get; set; }
    public bool WeatherDeclaredWet { get; set; }
    public float AirDensity { get; set; }
    public float AirPressure { get; set; }
    
    
}