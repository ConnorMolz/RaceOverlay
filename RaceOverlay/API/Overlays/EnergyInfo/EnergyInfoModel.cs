using RaceOverlay.Data.Models;

namespace RaceOverlay.API.Overlays.EnergyInfo;

public class EnergyInfoModel
{
    public float EnergyLevelPct { get; set; }

    public EnergyInfoModel()
    {
        iRacingData iRacingData = MainWindow.IRacingData;
        EnergyLevelPct = iRacingData.LocalCarTelemetry.EngeryLevelPct;
    }
}