namespace RaceOverlay.Data.Models;

public class LocalCarTelemetry
{
    public LocalCarTelemetry()
    {
        FrontLeftTyre = new Tyre();
        FrontRightTyre = new Tyre();
        RearLeftTyre = new Tyre();
        RearRightTyre = new Tyre();
        
        FrontLeftDamper = new Damper();
        FrontRightDamper = new Damper();
        RearLeftDamper = new Damper();
        RearRightDamper = new Damper();
    }
    
    //Tiers
    public Tyre FrontLeftTyre { get; set; }
    public Tyre FrontRightTyre { get; set; }
    public Tyre RearLeftTyre { get; set; }
    public Tyre RearRightTyre { get; set; }
    
    //Dampers
    public Damper FrontLeftDamper { get; set; }
    public Damper FrontRightDamper { get; set; }
    public Damper RearLeftDamper { get; set; }
    public Damper RearRightDamper { get; set; }
    
    
    public int CurrentRPM { get; set; }
    public int Gear { get; set; }
    public float Speed { get; set; }
    
    public float FuelLevel { get; set; }
  
    public float FuelCapacity { get; set; }
    public float OilTemp { get; set; }
    public float WaterTemp { get; set; }
    
    // Battery (GPT Only)
    public float EngeryLevelPct { get; set; } = 0;
    
    // Indy car Series Only
    public float P2PLeft { get; set; } = 0; // P2P = Push to Pass, only available in Indy car series
    public bool P2POn { get; set; } = false; // P2P = Push to Pass, only available in Indy car series   
    
    // Lap data
    public int Lap { get; set; }
    
    
    // Brakes
    public float BrakeBias { get; set; }
    
    // Car Electronics
    public float Tc1 { get; set; }
    public float Tc2 { get; set; }
    public float Abs { get; set; }
    public float ARBFront { get; set; }  //ARB = Anti Roll Bar
    public float ARBRear { get; set; }


}