namespace RaceOverlay.Data.Models;

public class iRacingData
{
    public LocalCarTelemetry LocalCarTelemetry { get; set; }
    public Inputs Inputs { get; set; }
    
    public SessionData SessionData { get; set; }
    public WeatherData WeatherData { get; set; }
}