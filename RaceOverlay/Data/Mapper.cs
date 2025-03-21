using System.Diagnostics;
using IRSDKSharper;
using RaceOverlay.Data.Models;

#pragma warning disable CS0168 // Variable is declared but never used

namespace RaceOverlay.Data;

public class Mapper
{
    public static iRacingData MapData(IRacingSdk irsdkSharper)
    {
        iRacingData data = new();
        
        // Getting Idx of the player
        data.PlayerIdx = irsdkSharper.Data.GetInt("PlayerCarIdx");
        
        // Map Session Data
        data.SessionData.TimeLeft = irsdkSharper.Data.GetDouble("SessionTimeRemain");
        data.SessionData.TimeTotal = irsdkSharper.Data.GetDouble("SessionTimeTotal");
        data.SessionData.LapsLeft = irsdkSharper.Data.GetInt("SessionLapsRemain");
        data.SessionData.LapsTotal = irsdkSharper.Data.GetInt("SessionLapsTotal");
        data.SessionData.LapsLeftEstimated = irsdkSharper.Data.GetInt("SessionLapsRemainEx");
        //Incidents
        try
        {
            data.SessionData.MaxIncidents =
                int.Parse(irsdkSharper.Data.SessionInfo.WeekendInfo.WeekendOptions.IncidentLimit);
        }
        catch (Exception)
        {
            data.SessionData.MaxIncidents = 0;
        }

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
            irsdkSharper.Data.GetFloat("LFwearL") * 100,
            irsdkSharper.Data.GetFloat("LFwearM") * 100,
            irsdkSharper.Data.GetFloat("LFwearR") * 100
            );
        data.LocalCarTelemetry.FrontRightTyre = new Tyre(
            irsdkSharper.Data.GetFloat("RFcoldPressure"),
            irsdkSharper.Data.GetFloat("RFtempCL"),
            irsdkSharper.Data.GetFloat("RFtempCM"),
            irsdkSharper.Data.GetFloat("RFtempCR"),
        irsdkSharper.Data.GetFloat("RFwearL") * 100,
        irsdkSharper.Data.GetFloat("RFwearM") * 100,
        irsdkSharper.Data.GetFloat("RFwearR") * 100
        );
        data.LocalCarTelemetry.RearLeftTyre = new Tyre(
            irsdkSharper.Data.GetFloat("LRcoldPressure"),
            irsdkSharper.Data.GetFloat("LRtempCL"),
            irsdkSharper.Data.GetFloat("LRtempCM"),
            irsdkSharper.Data.GetFloat("LRtempCR"),
            irsdkSharper.Data.GetFloat("LRwearL") * 100,
            irsdkSharper.Data.GetFloat("LRwearM") * 100,
            irsdkSharper.Data.GetFloat("LRwearR") * 100
        );
        data.LocalCarTelemetry.RearRightTyre = new Tyre(
            irsdkSharper.Data.GetFloat("RRcoldPressure"),
            irsdkSharper.Data.GetFloat("RRtempCL"),
            irsdkSharper.Data.GetFloat("RRtempCM"),
            irsdkSharper.Data.GetFloat("RRtempCR"),
            irsdkSharper.Data.GetFloat("RRwearL") * 100,
            irsdkSharper.Data.GetFloat("RRwearM") * 100,
            irsdkSharper.Data.GetFloat("RRwearR") * 100
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
        try
        {
            data.LocalCarTelemetry.FuelCapacity = irsdkSharper.Data.SessionInfo.DriverInfo.DriverCarFuelMaxLtr;
        }
        catch (Exception e)
        {
            //ignored
        }
       
        
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
        
        // Lap Deltas
        data.LocalDriver.LastLapDelta = irsdkSharper.Data.GetFloat("LapDeltaToSessionLastlLap");
        data.LocalDriver.BestLapDelta = irsdkSharper.Data.GetFloat("LapDeltaToSessionBestLap");
        
        // Drive Assistants
        try
        {
            data.LocalCarTelemetry.BrakeBias = irsdkSharper.Data.GetFloat("dcBrakeBias");
        }
        catch (Exception e)
        {
            // ignored
        }

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
        
        try
        {
            data.LocalCarTelemetry.ARBFront = irsdkSharper.Data.GetFloat("dcAntiRollFront");
        }
        catch (Exception e)
        {
            // ignored
        }
        
        try
        {
            data.LocalCarTelemetry.ARBRear = irsdkSharper.Data.GetFloat("dcAntiRollRear");
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
        data.WeatherData.Precipitation = irsdkSharper.Data.GetFloat("Precipitation") * 100;
        data.WeatherData.WindDir = irsdkSharper.Data.GetFloat("WindDir");
        data.WeatherData.WindVel = irsdkSharper.Data.GetFloat("WindVel");
        data.WeatherData.WeatherDeclaredWet = irsdkSharper.Data.GetBool("WeatherDeclaredWet");
        data.WeatherData.AirDensity = irsdkSharper.Data.GetFloat("AirDensity");
        data.WeatherData.AirPressure = irsdkSharper.Data.GetFloat("AirPressure");
        

        // Get if driver is on Track
        data.InCar = irsdkSharper.Data.GetBool("IsOnTrack");

        // Pitstop Data
        data.Pitstop.RequiredRepairTimeLeft = irsdkSharper.Data.GetFloat("PitRepairLeft");
        data.Pitstop.OptionalRepairTimeLeft = irsdkSharper.Data.GetFloat("PitOptRepairLeft");
        data.Pitstop.InPit = irsdkSharper.Data.GetBool("OnPitRoad");
        Debug.WriteLine(data.Pitstop.InPit);
        
        // Map Driver Data
        List<DriverModel> drivers = new List<DriverModel>();
        try
        {
            for (int i = 0; i < irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.Count; i++)
            {
                if (irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarIsPaceCar == 1)
                {
                    // Skip the Pace Car
                    continue;
                }
                drivers.Add(
                    new DriverModel(
                        irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).UserName,
                        irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).IRating,
                        irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).LicString,
                        irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarNumberRaw,
                        irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarClassShortName,
                        irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarScreenNameShort,
                        irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarIdx,
                        irsdkSharper.Data.GetFloat("CarIdxLapDistPct", irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarIdx) *100,
                        irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarClassEstLapTime,
                        irsdkSharper.Data.GetInt("CarIdxPosition", irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarIdx),
                        irsdkSharper.Data.GetInt("CarIdxClassPosition", irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarIdx),
                        irsdkSharper.Data.GetInt("CarIdxLap", irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarIdx),
                        irsdkSharper.Data.GetFloat("CarIdxLastLapTime", irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarIdx),
                        irsdkSharper.Data.GetFloat("CarIdxBestLapTime", irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarIdx),
                        irsdkSharper.Data.GetBool("CarIdxOnPitRoad", irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarIdx),
                        irsdkSharper.Data.GetFloat("CarIdxF2Time", irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarIdx),
                        irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarClassColor
                    )
                );
            }
        }
        catch (Exception e)
        {
            //ignored
        }
        
        data.Drivers = drivers.ToArray();
        data.InGarage = irsdkSharper.Data.GetBool("IsInGarage");
        
        // Return Dataset
        return data;
    }
    
}