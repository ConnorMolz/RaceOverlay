namespace RaceOverlay.Data.Models;

public class DriverModel
{
    public string Name { get; set; }
    public int iRating { get; set; }
    public string License {get; set;}
    public int CarNumber { get; set; }
    public int CarClass { get; set; }
    public string CarName { get; set; }
    
    public int Idx { get; set; }
    public float LapDistance { get; set; }
    public float EstCarClassNeededLapTime { get; set; }
    
    public int Position { get; set; }
    public int ClassPosition { get; set; }
    public int Lap { get; set; }
    public int LastLap { get; set; }
    public int BestLap { get; set; }

    public DriverModel(string name, int iRating, string license, int carNumber, int carClass, string carName, int idx, float lapDistance, float estCarClassNeededLapTime, int position, int classPosition, int lap, int lastLap, int bestLap)
    {
        Name = name;
        this.iRating = iRating;
        License = license;
        CarNumber = carNumber;
        CarClass = carClass;
        CarName = carName;
        Idx = idx;
        LapDistance = lapDistance;
        EstCarClassNeededLapTime = estCarClassNeededLapTime;
        Position = position;
        ClassPosition = classPosition;
        Lap = lap;
        LastLap = lastLap;
        BestLap = bestLap;
    }
}