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

    public int GetGapToPlayerMs(int index)
    {
        var _iRacingSDK = MainWindow.getRSDK();
        int playerCarIdx = PlayerIdx;
        float bestForPlayer = _iRacingSDK.Data.GetFloat("CarIdxBestLapTime", playerCarIdx);
        if (bestForPlayer == 0)
            bestForPlayer = _iRacingSDK.Data.SessionInfo.DriverInfo.Drivers[playerCarIdx].CarClassEstLapTime;

        float C = _iRacingSDK.Data.GetFloat("CarIdxEstTime", index);
        float S = _iRacingSDK.Data.GetFloat("CarIdxEstTime", playerCarIdx);

        // Does the delta between us and the other car span across the start/finish line?
        bool wrap = Math.Abs(_iRacingSDK.Data.GetFloat("CarIdxLapDistPct", index) -
                             _iRacingSDK.Data.GetFloat("CarIdxLapDistPct", playerCarIdx)) > 0.5f;
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


    public int GetGapToClassLeaderMS(int classLeaderIdx, int targetCarIdx)
    {
        var _iRacingSDK = MainWindow.getRSDK();
        float bestForLeader = _iRacingSDK.Data.GetFloat("CarIdxBestLapTime", classLeaderIdx);
        if (bestForLeader == 0)
            bestForLeader = _iRacingSDK.Data.SessionInfo.DriverInfo.Drivers[classLeaderIdx].CarClassEstLapTime;

        float C = _iRacingSDK.Data.GetFloat("CarIdxEstTime", targetCarIdx);
        float S = _iRacingSDK.Data.GetFloat("CarIdxEstTime", classLeaderIdx);

        float targetCarLapDist = _iRacingSDK.Data.GetFloat("CarIdxLapDistPct", targetCarIdx);
        float classLeaderLapDist = _iRacingSDK.Data.GetFloat("CarIdxLapDistPct", classLeaderIdx);
        float targetCarLap = _iRacingSDK.Data.GetFloat("CarIdxLap", targetCarIdx);
        float classLeaderLap = _iRacingSDK.Data.GetFloat("CarIdxLap", classLeaderIdx);

        if (targetCarLapDist < classLeaderLapDist && targetCarLap < classLeaderLap)
        {
            return (int)(targetCarLap - classLeaderLap);
        }

        // Does the delta between us and the other car span across the start/finish line?
        bool wrap = Math.Abs(targetCarLapDist - classLeaderLapDist) > 0.5f;
        float delta;
        if (wrap)
        {
            delta = S > C ? (C - S) + bestForLeader : (C - S) - bestForLeader;
            // lapDelta += S > C ? -1 : 1;
        }
        else
        {
            delta = C - S;
        }

        return (int)(delta * 1000);
    }
    
    public int GetGapBetweenMs(int driver1, int driver2)
    {
        var _iRacingSDK = MainWindow.getRSDK();
        float bestForDriver1 = _iRacingSDK.Data.GetFloat("CarIdxBestLapTime", driver1);
        if (bestForDriver1 == 0)
            bestForDriver1 = _iRacingSDK.Data.SessionInfo.DriverInfo.Drivers[driver1].CarClassEstLapTime;

        float C = _iRacingSDK.Data.GetFloat("CarIdxEstTime", driver2);
        float S = _iRacingSDK.Data.GetFloat("CarIdxEstTime", driver1);

        // Does the delta between us and the other car span across the start/finish line?
        bool wrap = Math.Abs(_iRacingSDK.Data.GetFloat("CarIdxLapDistPct", driver2) -
                             _iRacingSDK.Data.GetFloat("CarIdxLapDistPct", driver1)) > 0.5f;
        float delta;
        if (wrap)
        {
            delta = S > C ? (C - S) + bestForDriver1 : (C - S) - bestForDriver1;

            // lapDelta += S > C ? -1 : 1;
        }
        else
        {
            delta = C - S;
        }

        return (int)(delta * 1000);
    }
}