namespace RaceOverlay.Data.Models;

public class DriverModel
{
    public string Name { get; set; }
    public int iRating { get; set; }
    public string License {get; set;}
    public int CarNumber { get; set; }
    public string CarClass { get; set; }
    public string CarName { get; set; }
    
    public int Idx { get; set; }
    public float LapDistance { get; set; }
    public float EstCarClassNeededLapTime { get; set; }
    
    public int Position { get; set; }
    public int ClassPosition { get; set; }
    public int Lap { get; set; }
    public float LastLap { get; set; }
    public float BestLap { get; set; }
    public bool OnPitRoad { get; set; }
    public float GapToLeader { get; set; }

    public DriverModel(string name, int iRating, string license, int carNumber, string carClass, string carName, int idx, float lapDistance, float estCarClassNeededLapTime, int position, int classPosition, int lap, float lastLap, float bestLap, bool onPitRoad, float gapToLeader)
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
        OnPitRoad = onPitRoad;
        GapToLeader = gapToLeader;
        
    }
}