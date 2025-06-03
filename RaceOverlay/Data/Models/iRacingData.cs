using System.Diagnostics;
using IRSDKSharper;

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

    public int GetGapToPlayerMs(DriverModel player, DriverModel target)
    {
        double gapPercentage = 0;
        
        if (player.Lap == target.Lap)
        {
            gapPercentage = target.LapPtc - player.LapPtc;
        }
        else if (player.Lap > target.Lap)
        {
            gapPercentage = (1 - player.LapPtc) + target.LapPtc;
        }
        else if (player.Lap < target.Lap)
        {
            gapPercentage = (1 - target.LapPtc) + player.LapPtc;
        }
        
        double gapInSeconds = gapPercentage * target.EstLapTime;
        
        return (int)(gapInSeconds * 1000);
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
    
    public void AddOrUpdateDriver(int idx, IRacingSdk irsdkSharper)
    {
        int index = irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.FindIndex(d => d.CarIdx == idx);
        List<DriverModel>? newDrivers = null;
        var session = irsdkSharper.Data.SessionInfo.SessionInfo.Sessions
            .Find(d => d.SessionNum == irsdkSharper.Data.GetInt("SessionNum"));
        for (int i = 0; i < Drivers.Length; i++)
        {
            if (Drivers[i].Idx == idx)
            {
                var currentResult = session.ResultsPositions.Find(d => d.CarIdx == idx);
                Drivers[i].Lap = currentResult.Lap;
                Drivers[i].Position = currentResult.Position;
                Drivers[i].ClassPosition = currentResult.ClassPosition;
                Drivers[i].FastestLap = currentResult.FastestTime;
                Drivers[i].LastLap = currentResult.LastTime;
                Drivers[i].LapPtc = irsdkSharper.Data.GetFloat("CarIdxLapDistPct", idx);
                Drivers[i].OnPitRoad = irsdkSharper.Data.GetBool("CarIdxOnPitRoad", idx);
                return;
            }
        }

        // If we reach here, the driver was not found, so we add it
        try
        {
            var current = irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.Find(d => d.CarIdx == idx);
            var newDriver = new DriverModel(
                current.TeamName,
                current.IRating,
                current.LicString,
                current.CarClassColor,
                current.CarNumberRaw,
                current.CarClassEstLapTime,
                idx
            );
            newDrivers = new List<DriverModel>(Drivers) { newDriver };
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }

        if (newDrivers == null)
        {
            return;
        }
        Drivers = newDrivers.ToArray();
    }
}

