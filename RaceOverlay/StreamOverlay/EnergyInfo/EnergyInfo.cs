namespace RaceOverlay.StreamOverlay.EnergyInfo;

public class EnergyInfo: Internals.StreamOverlay
{
    public EnergyInfo(): base("Energy Info", "", "http://localhost:5480/overlay/energy_info")
    {
        
    }
}