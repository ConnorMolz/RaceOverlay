using System.Diagnostics;
using IRSDKSharper;
using RaceOverlay.Data.Models;
using RaceOverlay.Overlays.SessionInfo;

#pragma warning disable CS0168 // Variable is declared but never used

namespace RaceOverlay.Data;

[Flags]
public enum IrsdkFlags : uint
{
    None = 0x00000000,
    Checkered = 0x00000001,
    White = 0x00000002,
    Green = 0x00000004,
    Yellow = 0x00000008,
    Red = 0x00000010,
    Blue = 0x00000020,
    Debris = 0x00000040,
    Crossed = 0x00000080,
    YellowWaving = 0x00000100,
    OneLapToGreen = 0x00000200,
    GreenHeld = 0x00000400,
    TenToGo = 0x00000800,
    FiveToGo = 0x00001000,
    RandomWaving = 0x00002000,
    Caution = 0x00004000,
    CautionWaving = 0x00008000,
    Black = 0x00010000,
    Disqualify = 0x00020000,
    Servicible = 0x00040000,
    Furled = 0x00080000,
    Repair = 0x00100000,
    StartHidden = 0x10000000,
    StartReady = 0x20000000,
    StartSet = 0x40000000
}

public class Mapper
{
    public static double LogNumber { get; } = 1600 / Math.Log(2);
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
        data.Inputs.Steering = Single.RadiansToDegrees(irsdkSharper.Data.GetFloat("SteeringWheelAngle")) * -1;
        data.Inputs.Handbrake = irsdkSharper.Data.GetFloat("HandbrakeRaw"); // Todo: Need to check this
        
        
        // Map LocalCarTelemetry
        
        // Map Tyres
        data.LocalCarTelemetry.FrontLeftTyre = new Tyre(
            irsdkSharper.Data.GetFloat("LFtempCL"),
            irsdkSharper.Data.GetFloat("LFtempCM"),
            irsdkSharper.Data.GetFloat("LFtempCR"),
            irsdkSharper.Data.GetFloat("LFwearL") * 100,
            irsdkSharper.Data.GetFloat("LFwearM") * 100,
            irsdkSharper.Data.GetFloat("LFwearR") * 100
            );
        data.LocalCarTelemetry.FrontRightTyre = new Tyre(
            irsdkSharper.Data.GetFloat("RFtempCL"),
            irsdkSharper.Data.GetFloat("RFtempCM"),
            irsdkSharper.Data.GetFloat("RFtempCR"),
        irsdkSharper.Data.GetFloat("RFwearL") * 100,
        irsdkSharper.Data.GetFloat("RFwearM") * 100,
        irsdkSharper.Data.GetFloat("RFwearR") * 100
        );
        data.LocalCarTelemetry.RearLeftTyre = new Tyre(
            irsdkSharper.Data.GetFloat("LRtempCL"),
            irsdkSharper.Data.GetFloat("LRtempCM"),
            irsdkSharper.Data.GetFloat("LRtempCR"),
            irsdkSharper.Data.GetFloat("LRwearL") * 100,
            irsdkSharper.Data.GetFloat("LRwearM") * 100,
            irsdkSharper.Data.GetFloat("LRwearR") * 100
        );
        data.LocalCarTelemetry.RearRightTyre = new Tyre(
            irsdkSharper.Data.GetFloat("RRtempCL"),
            irsdkSharper.Data.GetFloat("RRtempCM"),
            irsdkSharper.Data.GetFloat("RRtempCR"),
            irsdkSharper.Data.GetFloat("RRwearL") * 100,
            irsdkSharper.Data.GetFloat("RRwearM") * 100,
            irsdkSharper.Data.GetFloat("RRwearR") * 100
        );
        
        // Map Dampers *Removed for performance resone because currently not needed
        /*data.LocalCarTelemetry.FrontLeftDamper = new Damper(
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
        );*/
        
        // Gear, RPM, Speed and Steering
        data.LocalCarTelemetry.CurrentRPM = irsdkSharper.Data.GetInt("RPM"); 
        data.LocalCarTelemetry.Gear = irsdkSharper.Data.GetInt("Gear");
        data.LocalCarTelemetry.Speed = irsdkSharper.Data.GetFloat("Speed") * 3.6f;
        
        // Fuel Level and Press
        data.LocalCarTelemetry.FuelLevel = irsdkSharper.Data.GetFloat("FuelLevel");
        try
        {
            data.LocalCarTelemetry.FuelCapacity = irsdkSharper.Data.SessionInfo.DriverInfo.DriverCarFuelMaxLtr;
        }
        catch (Exception e)
        {
            //ignored
        }
       
        
        // Oil Temp
        data.LocalCarTelemetry.OilTemp = irsdkSharper.Data.GetFloat("OilTemp");
        
        // Water Temp
        data.LocalCarTelemetry.WaterTemp = irsdkSharper.Data.GetFloat("WaterTemp");
        
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
        
        // P2P (Indycar Only)
        try
        {
            data.LocalCarTelemetry.P2PLeft = irsdkSharper.Data.GetFloat("P2P_Count");
            data.LocalCarTelemetry.P2POn = irsdkSharper.Data.GetBool("P2P_Status");
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
                        irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarIdx,
                        irsdkSharper.Data.GetInt("CarIdxPosition",
                            irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarIdx),
                        irsdkSharper.Data.GetInt("CarIdxClassPosition",
                            irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarIdx),
                        irsdkSharper.Data.GetInt("CarIdxLap",
                            irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarIdx),
                        irsdkSharper.Data.GetBool("CarIdxOnPitRoad",
                            irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarIdx),
                        irsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarClassColor
                    )
                );
            }

            data.SessionData.SOF = CalcSOF(drivers);
        }
        catch (Exception e)
        {
            //ignored
        }
        
        data.Drivers = drivers.ToArray();


        try
        {
            data.SessionData.SessionType = irsdkSharper.Data.SessionInfo.SessionInfo
                .Sessions[irsdkSharper.Data.GetInt("SessionNum")].SessionType;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }

        try
        {
            data.SessionData.InSimTime = irsdkSharper.Data.GetFloat("SessionTimeOfDay");
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }

        data.InGarage = irsdkSharper.Data.GetBool("IsInGarage");

        var Flag = irsdkSharper.Data.GetValue("SessionFlags");
        
        data.LocalDriver.CurrentIrsdkFlags = (IrsdkFlags)Flag;
        Console.WriteLine(data.LocalDriver.CurrentIrsdkFlags);
        
        // Return Dataset
        return data;
    }
    
    private static int CalcSOF(List<DriverModel> drivers)
    {
        double starters = drivers.Count();

        double sof = LogNumber * Math.Log(starters / drivers.Sum(r => Math.Exp(-r.iRating / LogNumber)));
        
        // Calculate the rating change for each driver Implement later
        /*foreach (var result in drivers)
        {
            var expectedScore = drivers.Sum(r => (1 - Math.Exp(-result.iRating / LogNumber))
                                                 * Math.Exp(-r.iRating / LogNumber)
                                                 / ((1 - Math.Exp(-r.iRating / LogNumber)) * Math.Exp(-result.iRating / LogNumber) + (1 - Math.Exp(-result.iRating / LogNumber))
                                                     * Math.Exp(-r.iRating / LogNumber))) - 0.5;

            var fudgeFactor = (starters / 2 - result.ClassPosition) / 100;

            result.RatingChange = (starters - result.ClassPosition - expectedScore - fudgeFactor) * 200 / starters;
        }*/
        
        return (int) sof;
    }
    
}