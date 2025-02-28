using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System;
using System.IO;
using Newtonsoft.Json;

namespace RaceOverlay;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    
    public static string AppDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "RaceOverlay");
    
    public App()
    {
        Debug.Print("Starting RaceOverlay...");
        CheckAppSettings();
    }

    private void CheckAppSettings()
    {
        // Get the local AppData path
        string appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RaceOverlay");
    
        // Create the directory if it doesn't exist
        if (!Directory.Exists(appDataPath))
        {
            Debug.WriteLine("AppData folder for RaceOverlay doesn't exist. Creating it now.");
            Directory.CreateDirectory(appDataPath);
        }
    
        // Define the path to settings.json
        string settingsFilePath = Path.Combine(appDataPath, "settings.json");
    
        // Check if settings.json exists, create it if not
        if (!File.Exists(settingsFilePath))
        {
            Debug.WriteLine("settings.json doesn't exist. Creating it with default values.");
        
            // Create default settings object
            var defaultSettings = new 
            {
                FirstRun = true,
                Overlays = new {}
            };
        
            // Serialize to JSON and write to file
            string jsonSettings = JsonConvert.SerializeObject(defaultSettings, Formatting.Indented);
            File.WriteAllText(settingsFilePath, jsonSettings);
        }
    }
    
}