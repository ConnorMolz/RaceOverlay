namespace RaceOverlay.Data.Models;

public class LocalCarTelemetry
{
    public Tyre FrontLeft { get; set; }
    public Tyre FrontRight { get; set; }
    public Tyre RearLeft { get; set; }
    public Tyre RearRight { get; set; }
    
    public int CurrentRPM { get; set; }
    public int Gear { get; set; }
    public float Speed { get; set; }
    
    public float FuelLevel { get; set; }
    public float FuelPressure { get; set; }
    
    public float OilTemp { get; set; }
    public float OilPressure { get; set; }
    public float OilLevel { get; set; }
    
    public float WaterTemp { get; set; }
    public float WaterLevel { get; set; }
    
    
    
}