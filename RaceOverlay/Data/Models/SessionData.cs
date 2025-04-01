namespace RaceOverlay.Data.Models;

public class SessionData
{
    public double TimeLeft { get; set; }
    public double TimeTotal { get; set; }
    public int LapsLeft { get; set; }
    public int LapsTotal { get; set; }
    public int LapsLeftEstimated { get; set; }
    public int MaxIncidents { get; set; }
    public int Incidents { get; set; }
    public int SOF { get; set; }
    public string SessionType { get; set; }
    
}