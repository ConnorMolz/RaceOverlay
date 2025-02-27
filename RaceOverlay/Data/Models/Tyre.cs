namespace RaceOverlay.Data.Models;

public class Tyre
{
    public float ColdPressure { get; set; }
    public float TemperatureLeft { get; set; }
    public float TemperatureCenter { get; set; }
    public float TemperatureRight { get; set; }
    public float WearLeft { get; set; }
    public float WearCenter { get; set; }
    public float WearRight { get; set; }
    
    public Tyre(
        float coldPressure, 
        float temperatureLeft, 
        float temperatureCenter, 
        float temperatureRight,
        float wearLeft,
        float wearCenter,
        float wearRight
        )
    {
        ColdPressure = coldPressure;
        TemperatureLeft = temperatureLeft;
        TemperatureCenter = temperatureCenter;
        TemperatureRight = temperatureRight;
        WearLeft = wearLeft;
        WearCenter = wearCenter;
        WearRight = wearRight;
    }
    
    public Tyre(){}
}