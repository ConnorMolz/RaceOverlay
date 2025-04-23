namespace RaceOverlay.StreamOverlay.Inputs;

public class Inputs: Internals.StreamOverlay
{
    public Inputs() : base("Inputs", 
        "Displays the current inputs, current gear and current speed of the car.", 
        "http://localhost:5480/overlay/inputs")
    {
    }
}