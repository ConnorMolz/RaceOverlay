namespace RaceOverlay.Data.Models;

public class DriverModel
{
    public string Name { get; set; }
    public int iRating { get; set; }
    public string License {get; set;}
    public int CarNumber { get; set; }
    public int Idx { get; set; }
    public int Position { get; set; }
    public int ClassPosition { get; set; }
    public int Lap { get; set; }
    public bool OnPitRoad { get; set; }
    public string ClassColorCode { get; set; }

    public DriverModel(string name, int iRating, string license, int carNumber, int idx, int position, int classPosition, int lap, bool onPitRoad, string classColor)
    {
        Name = name;
        this.iRating = iRating;
        License = license;
        CarNumber = carNumber;
        Idx = idx;
        Position = position;
        ClassPosition = classPosition;
        Lap = lap;
        OnPitRoad = onPitRoad;
        ClassColorCode = classColor;
    }

    public DriverModel()
    {
        
    }
}