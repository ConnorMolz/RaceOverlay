namespace RaceOverlay.Data.Models;

public class DriverModel
{
    public int Idx { get; set; }
    public string DriverName { get; set; }
    public int IRating { get; set; }
    public int LicenseLevel { get; set; }
    public int LicenseSubLevel { get; set; }
    
    
    
    public DriverModel(int idx, string driverName, int iRating, int licenseLevel, int licenseSubLevel)
    {
        Idx = idx;
        DriverName = driverName;
        IRating = iRating;
        LicenseLevel = licenseLevel;
        LicenseSubLevel = licenseSubLevel;
    }
}