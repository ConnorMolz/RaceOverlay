namespace RaceOverlay.StreamOverlay.EnergyInfo;

public class EnergyInfo: Internals.StreamOverlay
{
    public EnergyInfo(): base("Energy Info", 
        "Displays the current energy level of the battery. (Only available in GPT)", 
        "http://localhost:5480/overlay/energy_info")
    {
        
    }
}