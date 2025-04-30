namespace RaceOverlay.StreamOverlay.LaptimeDelta;

public class BestLaptimeDelta: Internals.StreamOverlay
{
    public BestLaptimeDelta(): base(
        "Best Lap Time Delta", 
        "This Overlay shows the current delta to the best lap time which is currently set by the local car.",
        "http://localhost:5480/overlay/best-lap"){}
}