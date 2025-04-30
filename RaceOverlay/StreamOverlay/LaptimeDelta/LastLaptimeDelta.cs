namespace RaceOverlay.StreamOverlay.LaptimeDelta;

public class LastLaptimeDelta: Internals.StreamOverlay
{
    public LastLaptimeDelta(): base("Last Lap Time Delta", 
        "This Overlay shows the current delta to the last lap time which is currently set by the local car.",
        "http://localhost:5480/overlay/last-lap"){}
}