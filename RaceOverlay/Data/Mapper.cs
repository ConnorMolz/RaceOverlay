using IRSDKSharper;
using RaceOverlay.Data.Models;

#pragma warning disable CS0168 // Variable is declared but never used

namespace RaceOverlay.Data;

public class Mapper
{
    public static iRacingData MapData(IRacingSdk irsdkSharper)
    {
        iRacingData data = new();
        
        // Map Session Data
        data.SessionData.TimeLeft = irsdkSharper.Data.GetFloat("SessionTimeRemain");
        data.SessionData.TimeTotal = irsdkSharper.Data.GetFloat("SessionTimeTotal");
        data.SessionData.LapsLeft = irsdkSharper.Data.GetInt("SessionLapsRemain");
        data.SessionData.LapsTotal = irsdkSharper.Data.GetInt("SessionLapsTotal");
        data.SessionData.LapsLeftEstimated = irsdkSharper.Data.GetInt("SessionLapsRemainEx");
        //Incidents
        data.SessionData.MaxIncidents = int.Parse(irsdkSharper.Data.SessionInfo.WeekendInfo.WeekendOptions.IncidentLimit);
        data.SessionData.Incidents = irsdkSharper.Data.GetInt("PlayerCarMyIncidentCount");
        
        // Map Inputs
        data.Inputs.Clutch = irsdkSharper.Data.GetFloat("Clutch");
        data.Inputs.Throttle = irsdkSharper.Data.GetFloat("Throttle");
        data.Inputs.Brake = irsdkSharper.Data.GetFloat("Brake");
        data.Inputs.Steering = irsdkSharper.Data.GetFloat("SteeringWheelAngle");
        data.Inputs.Handbrake = irsdkSharper.Data.GetFloat("HandbrakeRaw"); // Todo: Need to check this
        
        // Map LocalCarTelemetry
        
        // Map Tyres
        data.LocalCarTelemetry.FrontLeftTyre = new Tyre(
            irsdkSharper.Data.GetFloat("LFcoldPressure"),
            irsdkSharper.Data.GetFloat("LFtempCL"),
            irsdkSharper.Data.GetFloat("LFtempCM"),
            irsdkSharper.Data.GetFloat("LFtempCR"),
            irsdkSharper.Data.GetFloat("LFwearL"),
            irsdkSharper.Data.GetFloat("LFwearM"),
            irsdkSharper.Data.GetFloat("LFwearR")
            );
        data.LocalCarTelemetry.FrontRightTyre = new Tyre(
            irsdkSharper.Data.GetFloat("RFcoldPressure"),
            irsdkSharper.Data.GetFloat("RFtempCL"),
            irsdkSharper.Data.GetFloat("RFtempCM"),
            irsdkSharper.Data.GetFloat("RFtempCR"),
        irsdkSharper.Data.GetFloat("RFwearL"),
        irsdkSharper.Data.GetFloat("RFwearM"),
        irsdkSharper.Data.GetFloat("RFwearR")
        );
        data.LocalCarTelemetry.RearLeftTyre = new Tyre(
            irsdkSharper.Data.GetFloat("LRcoldPressure"),
            irsdkSharper.Data.GetFloat("LRtempCL"),
            irsdkSharper.Data.GetFloat("LRtempCM"),
            irsdkSharper.Data.GetFloat("LRtempCR"),
            irsdkSharper.Data.GetFloat("LRwearL"),
            irsdkSharper.Data.GetFloat("LRwearM"),
            irsdkSharper.Data.GetFloat("LRwearR")
        );
        data.LocalCarTelemetry.RearRightTyre = new Tyre(
            irsdkSharper.Data.GetFloat("RRcoldPressure"),
            irsdkSharper.Data.GetFloat("RRtempCL"),
            irsdkSharper.Data.GetFloat("RRtempCM"),
            irsdkSharper.Data.GetFloat("RRtempCR"),
            irsdkSharper.Data.GetFloat("RRwearL"),
            irsdkSharper.Data.GetFloat("RRwearM"),
            irsdkSharper.Data.GetFloat("RRwearR")
        );
        
        // Map Dampers
        data.LocalCarTelemetry.FrontLeftDamper = new Damper(
            irsdkSharper.Data.GetFloat("LFshockDefl"),
            irsdkSharper.Data.GetFloat("LFshockDefl_ST"),
            irsdkSharper.Data.GetFloat("LFshockVel"),
            irsdkSharper.Data.GetFloat("LFshockVel_ST")
        );
        data.LocalCarTelemetry.FrontRightDamper = new Damper(
            irsdkSharper.Data.GetFloat("RFshockDefl"),
            irsdkSharper.Data.GetFloat("RFshockDefl_ST"),
            irsdkSharper.Data.GetFloat("RFshockVel"),
            irsdkSharper.Data.GetFloat("RFshockVel_ST")
        );
        data.LocalCarTelemetry.RearLeftDamper = new Damper(
            irsdkSharper.Data.GetFloat("LRshockDefl"),
            irsdkSharper.Data.GetFloat("LRshockDefl_ST"),
            irsdkSharper.Data.GetFloat("LRshockVel"),
            irsdkSharper.Data.GetFloat("LRshockVel_ST")
        );
        data.LocalCarTelemetry.RearRightDamper = new Damper(
            irsdkSharper.Data.GetFloat("RRshockDefl"),
            irsdkSharper.Data.GetFloat("RRshockDefl_ST"),
            irsdkSharper.Data.GetFloat("RRshockVel"),
            irsdkSharper.Data.GetFloat("RRshockVel_ST")
        );
        
        // Gear, RPM, Speed
        data.LocalCarTelemetry.CurrentRPM = irsdkSharper.Data.GetInt("RPM"); 
        data.LocalCarTelemetry.Gear = irsdkSharper.Data.GetInt("Gear");
        data.LocalCarTelemetry.Speed = irsdkSharper.Data.GetFloat("Speed") * 3.6f;
        
        // Fuel Level and Press
        data.LocalCarTelemetry.FuelLevel = irsdkSharper.Data.GetFloat("FuelLevel");
        data.LocalCarTelemetry.FuelPressure = irsdkSharper.Data.GetFloat("FuelPress");
        
        // Oil Temp, Press and level
        data.LocalCarTelemetry.OilTemp = irsdkSharper.Data.GetFloat("OilTemp");
        data.LocalCarTelemetry.OilPressure = irsdkSharper.Data.GetFloat("OilPress");
        data.LocalCarTelemetry.OilLevel = irsdkSharper.Data.GetFloat("OilLevel");
        
        // Water Temp and level
        data.LocalCarTelemetry.WaterTemp = irsdkSharper.Data.GetFloat("WaterTemp");
        data.LocalCarTelemetry.WaterLevel = irsdkSharper.Data.GetFloat("WaterLevel");
        
        // Energy Level (GPT Only)
        try
        {
            // iRacing is expose a value between 1 and 0 by multiple with 100 the value is in Percent
            data.LocalCarTelemetry.EngeryLevelPct = irsdkSharper.Data.GetFloat("EnergyERSBatteryPct") * 100;
        }
        catch (Exception e)
        {
            // ignored
        }
        
        
        // Lap Data
        data.LocalCarTelemetry.Lap = irsdkSharper.Data.GetInt("Lap");
        
        // Drive Assistants
        data.LocalCarTelemetry.BrakeBias = irsdkSharper.Data.GetFloat("dcBrakeBias");
        try
        {
            data.LocalCarTelemetry.Tc1 = irsdkSharper.Data.GetFloat("dcTractionControl");
        }
        catch (Exception e)
        {
            // ignored
        }

        try
        {
            data.LocalCarTelemetry.Tc2 = irsdkSharper.Data.GetFloat("dcTractionControl2");
        }
        catch (Exception e)
        {
            // ignored
        }

        try
        {
            data.LocalCarTelemetry.Abs = irsdkSharper.Data.GetFloat("dcABS");
        }
        catch (Exception e)
        {
            // ignored
        }






        // Map Weather Data
        data.WeatherData.AirTemp = irsdkSharper.Data.GetFloat("AirTemp");
        data.WeatherData.TrackTemp = irsdkSharper.Data.GetFloat("TrackTemp");
        data.WeatherData.RelativeHumidity = irsdkSharper.Data.GetFloat("RelativeHumidity");
        data.WeatherData.FogLevel = irsdkSharper.Data.GetFloat("FogLevel");
        data.WeatherData.TrackTempCrew = irsdkSharper.Data.GetFloat("TrackTempCrew");
        data.WeatherData.Skies = irsdkSharper.Data.GetFloat("Skies");
        data.WeatherData.TrackWetness = irsdkSharper.Data.GetFloat("TrackWetness");
        data.WeatherData.Precipitation = irsdkSharper.Data.GetFloat("Precipitation");
        data.WeatherData.WindDir = irsdkSharper.Data.GetFloat("WindDir");
        data.WeatherData.WindVel = irsdkSharper.Data.GetFloat("WindVel");
        data.WeatherData.WeatherDeclaredWet = irsdkSharper.Data.GetBool("WeatherDeclaredWet");
        data.WeatherData.AirDensity = irsdkSharper.Data.GetFloat("AirDensity");
        data.WeatherData.AirPressure = irsdkSharper.Data.GetFloat("AirPressure");
        
        // Return Dataset
        return data;
    }
    
}