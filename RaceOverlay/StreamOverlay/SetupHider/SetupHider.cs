namespace RaceOverlay.StreamOverlay.SetupHider;

public class SetupHider: Internals.StreamOverlay
{
    public SetupHider() : base("Setup Hider", 
        "This Overlay provides an hider for setups while car is in Garage.", 
        "http://localhost:5480/overlay/setup_hider")
    {
    }
}