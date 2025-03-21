using Path = System.IO.Path;

namespace RaceOverlay.API.Overlays.SetupHider;

public class SetupHiderModel
{
    public bool InGarage { get; set; }
    public string ImagePath { get; set; }
    
    public SetupHiderModel()
    {
        InGarage = MainWindow.IRacingData.InGarage;
        ImagePath = Path.Combine(App.AppDataPath, "SetupHider.jpg");
    }
}