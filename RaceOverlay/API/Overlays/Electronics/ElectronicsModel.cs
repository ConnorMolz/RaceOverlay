using RaceOverlay.Data.Models;

namespace RaceOverlay.API.Overlays.Electronics;

public class ElectronicsModel
{
    public int abs_value { get; set; }
    public int tc1 { get; set; }
    public int tc2 { get; set; }
    public int bb { get; set; }
    public int abrf { get; set; }
    public int abrr { get; set; }
    
    public ElectronicsModel()
    {
        iRacingData iRacingData = MainWindow.IRacingData;
        abs_value = (int) iRacingData.LocalCarTelemetry.Abs;
        tc1 = (int) iRacingData.LocalCarTelemetry.Tc1;
        tc2 = (int) iRacingData.LocalCarTelemetry.Tc2;
        bb = (int) iRacingData.LocalCarTelemetry.BrakeBias;
        abrf = (int) iRacingData.LocalCarTelemetry.ARBFront;
        abrr = (int) iRacingData.LocalCarTelemetry.ARBRear;
    }
    
}