namespace RaceOverlay.Data.Models;

public class Damper
{
    public Damper(float shockDeflection, float schockDeflectionSt, float shockVelocity, float shockVelocitySt)
    {
        ShockDeflection = shockDeflection;
        SchockDeflectionST = schockDeflectionSt;
        ShockVelocity = shockVelocity;
        ShockVelocityST = shockVelocitySt;
    }

    private float ShockDeflection { get; set; }
    private float SchockDeflectionST { get; set; }
    float ShockVelocity { get; set; }
    float ShockVelocityST { get; set; }
    
    
    
}