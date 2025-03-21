using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System;
using System.IO;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RaceOverlay.API;

namespace RaceOverlay;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    
    public static string AppDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "RaceOverlay");
    
    IHost? _apiHost;
    
    public App()
    {
        Debug.Print("Starting RaceOverlay...");
        CheckAppSettings();
        CheckForFirstRun();
        _apiHost = StartAPI.StartApiServer();
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

    private void CheckForFirstRun()
    {
        string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
        string jsonContent = File.ReadAllText(settingsFilePath);
        JObject settingsObject = JObject.Parse(jsonContent);
        if(settingsObject["FirstRun"].Value<bool>())
        {
            Debug.WriteLine("First run detected. Opening FirstStartPage.");
            
            FirstStartPage firstStartPage = new();
            firstStartPage.Show();
        }
    }
    
    protected override async void OnExit(ExitEventArgs e)
    {
        if (_apiHost != null)
        {
            await _apiHost.StopAsync();
            _apiHost.Dispose();
        }
        base.OnExit(e);
    }
    
}