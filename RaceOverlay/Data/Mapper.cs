using HerboldRacing;
using RaceOverlay.Data.Models;

namespace RaceOverlay.Data;

public class Mapper
{
    public static iRacingData MapData(IRSDKSharper irsdkSharper)
    {
        iRacingData data = new();
        
        // Map Session Data
        data.SessionData.TimeLeft = irsdkSharper.Data.GetFloat("SessionTimeRemain");
        data.SessionData.TimeTotal = irsdkSharper.Data.GetFloat("SessionTimeTotal");
        data.SessionData.LapsLeft = irsdkSharper.Data.GetInt("SessionLapsRemain");
        data.SessionData.LapsTotal = irsdkSharper.Data.GetInt("SessionLapsTotal");
        data.SessionData.LapsLeftEstimated = irsdkSharper.Data.GetInt("SessionLapsRemainEx");
        
        // Map Inputs
        data.Inputs.Clutch = irsdkSharper.Data.GetFloat("Clutch");
        data.Inputs.Throttle = irsdkSharper.Data.GetFloat("Throttle");
        data.Inputs.Brake = irsdkSharper.Data.GetFloat("Brake");
        data.Inputs.Steering = irsdkSharper.Data.GetFloat("SteeringWheelAngle");
        data.Inputs.Handbrake = irsdkSharper.Data.GetFloat("HandbrakeRaw"); // Todo: Need to check this
        
        // Map LocalCarTelemetry
        
        // Map Tyres
        data.LocalCarTelemetry.FrontLeft = new Tyre(
            irsdkSharper.Data.GetFloat("LFcoldPressure"),
            irsdkSharper.Data.GetFloat("LFtempCL"),
            irsdkSharper.Data.GetFloat("LFtempCM"),
            irsdkSharper.Data.GetFloat("LFtempCR"),
            irsdkSharper.Data.GetFloat("LFwearL"),
            irsdkSharper.Data.GetFloat("LFwearC"),
            irsdkSharper.Data.GetFloat("LFwearR")
            );
        data.LocalCarTelemetry.FrontRight = new Tyre(
            irsdkSharper.Data.GetFloat("RFcoldPressure"),
            irsdkSharper.Data.GetFloat("RFtempCL"),
            irsdkSharper.Data.GetFloat("RFtempCM"),
            irsdkSharper.Data.GetFloat("RFtempCR"),
        irsdkSharper.Data.GetFloat("RFwearL"),
        irsdkSharper.Data.GetFloat("RFwearC"),
        irsdkSharper.Data.GetFloat("RFwearR")
        );
        data.LocalCarTelemetry.RearLeft = new Tyre(
            irsdkSharper.Data.GetFloat("LRcoldPressure"),
            irsdkSharper.Data.GetFloat("LRtempCL"),
            irsdkSharper.Data.GetFloat("LRtempCM"),
            irsdkSharper.Data.GetFloat("LRtempCR"),
            irsdkSharper.Data.GetFloat("LRwearL"),
            irsdkSharper.Data.GetFloat("LRwearC"),
            irsdkSharper.Data.GetFloat("LRwearR")
        );
        data.LocalCarTelemetry.RearRight = new Tyre(
            irsdkSharper.Data.GetFloat("RRcoldPressure"),
            irsdkSharper.Data.GetFloat("RRtempCL"),
            irsdkSharper.Data.GetFloat("RRtempCM"),
            irsdkSharper.Data.GetFloat("RRtempCR"),
            irsdkSharper.Data.GetFloat("RRwearL"),
            irsdkSharper.Data.GetFloat("RRwearC"),
            irsdkSharper.Data.GetFloat("RRwearR")
        );
        
        // Gear, RPM, Speed
        data.LocalCarTelemetry.CurrentRPM = irsdkSharper.Data.GetInt("RPM"); 
        data.LocalCarTelemetry.Gear = irsdkSharper.Data.GetInt("Gear");
        data.LocalCarTelemetry.Speed = irsdkSharper.Data.GetFloat("Speed");
        
        // FUel Level and Press
        data.LocalCarTelemetry.FuelLevel = irsdkSharper.Data.GetFloat("FuelLevel");
        data.LocalCarTelemetry.FuelPressure = irsdkSharper.Data.GetFloat("FuelPress");
        
        // Oil Temp, Press and level
        data.LocalCarTelemetry.OilTemp = irsdkSharper.Data.GetFloat("OilTemp");
        data.LocalCarTelemetry.OilPressure = irsdkSharper.Data.GetFloat("OilPress");
        data.LocalCarTelemetry.OilLevel = irsdkSharper.Data.GetFloat("OilLevel");
        
        // Water Temp and level
        data.LocalCarTelemetry.WaterTemp = irsdkSharper.Data.GetFloat("WaterTemp");
        data.LocalCarTelemetry.WaterLevel = irsdkSharper.Data.GetFloat("WaterLevel");
        return data;
    }
}