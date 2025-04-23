using RaceOverlay.Data.Models;

namespace RaceOverlay.API.Overlays.EnergyInfo;

public class EnergyModel
{
    public float EnergyLevelPct { get; set; }

    public EnergyModel()
    {
        iRacingData iRacingData = MainWindow.IRacingData;
        EnergyLevelPct = iRacingData.LocalCarTelemetry.EngeryLevelPct;
    }
}