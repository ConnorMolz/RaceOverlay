using System.IO;
using Path = System.IO.Path;

namespace RaceOverlay.API.Overlays.SetupHider;

public class SetupHiderModel
{
    public bool InGarage { get; set; }
    public string ImagePath { get; set; }
    
    public SetupHiderModel()
    {
        InGarage = MainWindow.IRacingData.InGarage;
        var imagePath = Path.Combine(App.AppDataPath, "SetupHider.jpg");
        ImagePath = ConvertImageToBase64(imagePath);
    }

    private string ConvertImageToBase64(string imagePath)
    {
        if (!File.Exists(imagePath))
            return string.Empty;
            
        byte[] imageBytes = File.ReadAllBytes(imagePath);
        return $"data:image/jpeg;base64,{Convert.ToBase64String(imageBytes)}";
    }
}