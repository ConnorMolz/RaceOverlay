namespace RaceOverlay.Internals;

public class OverlayConfig
{
    public string OverlayName { get; set; }
    public List<Config>? ConfigData { get; set; }
    
    
    
    public class Config
    {
        public string Key { get; set; }
        public string DataType { get; set; }
        public string Value { get; set; }
    }
}