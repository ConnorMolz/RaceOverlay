namespace RaceOverlay.Data.Models;

public class iRacingData
{
    public LocalCarTelemetry LocalCarTelemetry { get; set; }
    public Inputs Inputs { get; set; }
    
    public SessionData SessionData { get; set; }
    public WeatherData WeatherData { get; set; }
    

    public LocalDriver LocalDriver { get; set; }
    public Pitstop Pitstop { get; set; }
  
    public bool InCar { get; set; }
    public DriverModel[] Drivers { get; set; }
    public int PlayerIdx { get; set; }
  
    public iRacingData()
    {
        LocalCarTelemetry = new LocalCarTelemetry();
        Inputs = new Inputs();
        SessionData = new SessionData();
        WeatherData = new WeatherData();
        LocalDriver = new LocalDriver();
        Pitstop = new Pitstop();
        Drivers = [];
    }
}