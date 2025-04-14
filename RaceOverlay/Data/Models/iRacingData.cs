namespace RaceOverlay.Data.Models;

public class iRacingData
{
    public LocalCarTelemetry LocalCarTelemetry { get; set; }
    public Inputs Inputs { get; set; }
    
    public SessionData SessionData { get; set; }
    public WeatherData WeatherData { get; set; }
    

    public LocalDriver LocalDriver { get; set; }
    public Pitstop Pitstop { get; set; }
  
    public bool InCar { get; set; }
    public DriverModel[] Drivers { get; set; }
    public int PlayerIdx { get; set; }
    public bool InGarage { get; set; }
    public iRacingData()
    {
        LocalCarTelemetry = new LocalCarTelemetry();
        Inputs = new Inputs();
        SessionData = new SessionData();
        WeatherData = new WeatherData();
        LocalDriver = new LocalDriver();
        Pitstop = new Pitstop();
        Drivers = [];
    }
    
    public DriverModel GetDriverByIdx(int idx)
    {
        foreach (var driver in Drivers)
        {
            if (driver.Idx == idx)
            {
                return driver;
            }
        }
        return null;
    }
    
    public int GetGapToPlayerMs(int index )
    {
        var _iRacingSDK = MainWindow.getRSDK();
        int playerCarIdx = PlayerIdx;
        float bestForPlayer = _iRacingSDK.Data.GetFloat("CarIdxBestLapTime", playerCarIdx);
        if (bestForPlayer == 0)
            bestForPlayer = _iRacingSDK.Data.SessionInfo.DriverInfo.Drivers[playerCarIdx].CarClassEstLapTime;

        float C = _iRacingSDK.Data.GetFloat("CarIdxEstTime", index);
        float S = _iRacingSDK.Data.GetFloat("CarIdxEstTime", playerCarIdx);

        // Does the delta between us and the other car span across the start/finish line?
        bool wrap = Math.Abs(_iRacingSDK.Data.GetFloat("CarIdxLapDistPct", index) - _iRacingSDK.Data.GetFloat("CarIdxLapDistPct", playerCarIdx)) > 0.5f;
        float delta;
        if (wrap)
        {
            delta = S > C ? (C - S) + bestForPlayer : (C - S) - bestForPlayer;
            // lapDelta += S > C ? -1 : 1;
        }
        else
        {
            delta = C - S;
        }
        return (int)(delta * 1000);
    }
    
}