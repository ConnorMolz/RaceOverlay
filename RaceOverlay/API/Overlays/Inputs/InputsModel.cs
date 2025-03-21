namespace API_test.Overlays.Inputs;

public class InputsModel
{
    public double Throttle { get; set; }
    public double Brake { get; set; }
    public double Clutch { get; set; }
    public int Gear { get; set; }
    public double Speed { get; set; }
    
    public InputsModel(double throttle, double brake, double clutch, int gear, double speed)
    {
        Throttle = throttle;
        Brake = brake;
        Clutch = clutch;
        Gear = gear;
        Speed = speed;
    }
    
    public InputsModel()
    {
        Throttle = 0;
        Brake = 0;
        Clutch = 0;
        Gear = 0;
        Speed = 0;
    }
}