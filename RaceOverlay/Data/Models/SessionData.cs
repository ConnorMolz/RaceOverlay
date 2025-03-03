namespace RaceOverlay.Data.Models;

public class SessionData
{
    public float TimeLeft { get; set; }
    public float TimeTotal { get; set; }
    public int LapsLeft { get; set; }
    public int LapsTotal { get; set; }
    public int LapsLeftEstimated { get; set; }
    public int MaxIncidents { get; set; }
    public int Incidents { get; set; }
    
}