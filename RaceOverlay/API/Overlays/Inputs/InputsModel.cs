using RaceOverlay;
using RaceOverlay.Data.Models;

namespace API_test.Overlays.Inputs;

public class InputsModel
{
    public double Throttle { get; set; }
    public double Brake { get; set; }
    public double Clutch { get; set; }
    public int Gear { get; set; }
    public double Speed { get; set; }
    
    public InputsModel()
    {
        iRacingData IRacingData = MainWindow.IRacingData;
        Throttle = IRacingData.Inputs.Throttle;
        Brake = IRacingData.Inputs.Brake;
        Clutch = 1 - IRacingData.Inputs.Clutch;
        Gear = IRacingData.LocalCarTelemetry.Gear;
        Speed = IRacingData.LocalCarTelemetry.Speed;
    }
}