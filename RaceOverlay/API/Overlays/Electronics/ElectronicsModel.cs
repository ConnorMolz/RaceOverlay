using RaceOverlay.Data.Models;

namespace RaceOverlay.API.Overlays.Electronics;

public class ElectronicsModel
{
    public int abs_value { get; set; }
    public int tc1 { get; set; }
    public int tc2 { get; set; }
    public float bb { get; set; }
    public int abrf { get; set; }
    public int abrr { get; set; }
    public bool show_abs { get; set; }
    public bool show_tc1 { get; set; }
    public bool show_tc2 { get; set; }
    public bool show_brake_bias { get; set; }
    public bool show_arb_front { get; set; }
    public bool show_arb_rear { get; set; }
    
    public ElectronicsModel()
    {
        iRacingData iRacingData = MainWindow.IRacingData;
        abs_value = (int) iRacingData.LocalCarTelemetry.Abs;
        tc1 = (int) iRacingData.LocalCarTelemetry.Tc1;
        tc2 = (int) iRacingData.LocalCarTelemetry.Tc2;
        bb = iRacingData.LocalCarTelemetry.BrakeBias;
        abrf = (int) iRacingData.LocalCarTelemetry.ARBFront;
        abrr = (int) iRacingData.LocalCarTelemetry.ARBRear;
        show_abs = StreamOverlay.Electronics.Electronics.ShowABS;
        show_tc1 = StreamOverlay.Electronics.Electronics.ShowTC1;
        show_tc2 = StreamOverlay.Electronics.Electronics.ShowTC2;
        show_brake_bias = StreamOverlay.Electronics.Electronics.ShowBrakeBias;
        show_arb_front = StreamOverlay.Electronics.Electronics.ShowARBFront;
        show_arb_rear = StreamOverlay.Electronics.Electronics.ShowARBRear;
    }
    
}