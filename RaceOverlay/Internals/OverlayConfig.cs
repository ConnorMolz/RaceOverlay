namespace RaceOverlay.Internals;

public class OverlayConfig
{
    public string OverlayName { get; set; }
    public List<Config>? ConfigData { get; set; }
    
    
    
    public class Config
    {
        public string Key { get; set; }
        public string? StringValue { get; set; }
        public int? IntValue { get; set; }
        public bool? BoolValue { get; set; }
        public float? FloatValue { get; set; }
        public double? DoubleValue { get; set; }
        public List<string>? StringListValue { get; set; }
        public List<int>? IntListValue { get; set; }
        public List<bool>? BoolListValue { get; set; }
        public List<float>? FloatListValue { get; set; }
    }
}