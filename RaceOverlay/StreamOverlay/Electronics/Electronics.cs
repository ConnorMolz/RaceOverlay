namespace RaceOverlay.StreamOverlay.Electronics;

public class Electronics: Internals.StreamOverlay
{
    public Electronics() : base("Electronics", 
        "An Overlay for displaying the in car adjustments of ABS, TC1, TC2, Brake Bias(BB) and Anit Roll Bars (ARB) Front and Rear.", 
        "http://localhost:5480/overlay/electronics")
    {
    }
}