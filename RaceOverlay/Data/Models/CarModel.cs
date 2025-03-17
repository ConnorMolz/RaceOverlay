namespace RaceOverlay.Data.Models;

public class CarModel
{
    public DriverModel[] Drivers { get; set; }
    public String CarName { get; set; }
    public string CarNumber { get; set; }
    public int CarIdx { get; set; }
}