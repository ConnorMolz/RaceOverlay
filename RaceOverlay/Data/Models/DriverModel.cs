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
    public float LapPtc { get; set; }
    public double RatingChange { get; set; }
    public double FastestLap { get; set; }
    public double LastLap { get; set; }

    public DriverModel(string name, int iRating, string license, string classColor, int carNumber, int idx)
    {
        Name = name;
        this.iRating = iRating;
        License = license;
        ClassColorCode = classColor;
        CarNumber = carNumber;
        Idx = idx;
    }

    public DriverModel()
    {
        
    }
}