namespace RaceOverlay.Data.Models;

public class LocalCarTelemetry
{
    //Tiers
    public Tyre FrontLeftTyre { get; set; }
    public Tyre FrontRightTyre { get; set; }
    public Tyre RearLeftTyre { get; set; }
    public Tyre RearRightTyre { get; set; }
    
    
    
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