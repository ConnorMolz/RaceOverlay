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
/// Entry point for the RaceOverlay application.
/// Initializes application settings, checks for first run, and starts the API service.
/// 
/// </summary>
public partial class App : Application
{

    public static string AppDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "RaceOverlay");

    static IHost? _apiHost;

    public App()
    {
        Debug.Print("Starting RaceOverlay...");
        CheckAppSettings();
        InitSetupHiderImage();
        CheckForFirstRun();

    }

    /// <summary>
    /// Check if the application config file and folder is existing and create it if not.
    /// </summary>
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
                Overlays = new { }
            };

            // Serialize to JSON and write to file
            string jsonSettings = JsonConvert.SerializeObject(defaultSettings, Formatting.Indented);
            File.WriteAllText(settingsFilePath, jsonSettings);
        }
    }

    /// <summary>
    /// Checks if this is the first run of the application by reading settings.json.
    /// </summary>
    private void CheckForFirstRun()
    {
        string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
        string jsonContent = File.ReadAllText(settingsFilePath);
        JObject settingsObject = JObject.Parse(jsonContent);
        if (settingsObject["FirstRun"].Value<bool>())
        {
            Debug.WriteLine("First run detected. Opening FirstStartPage.");

            FirstStartPage firstStartPage = new();
            firstStartPage.Show();
        }
    }

    /// <summary>
    /// Stops the API service when the application exits for an orderly shutdown.
    /// </summary>
    /// <param name="e"></param>
    protected override async void OnExit(ExitEventArgs e)
    {
        if (_apiHost != null)
        {
            await _apiHost.StopAsync();
            _apiHost.Dispose();
        }

        base.OnExit(e);
    }

    /// <summary>
    /// Check if the SetupHider image exists in the AppData folder and copy the default Image if no image is avialble.
    /// </summary>
    private void InitSetupHiderImage()
    {
        // Define the path to settings.json
        string filePath = Path.Combine(AppDataPath, "SetupHider.jpg");

        // Check if the file exists, create it if not
        if (!File.Exists(filePath))
        {
            var assembly = typeof(App).Assembly;
            var stream = assembly.GetManifestResourceStream("RaceOverlay.Resources.SetupHider.jpg");
            Debug.WriteLine("SetupHider.jpg doesn't exist. Creating it with default values.");
            File.WriteAllBytes(filePath, new BinaryReader(stream).ReadBytes((int)stream.Length));
        }
    }

    /// <summary>
    /// Creating a new thread to start the API service decoupled from the UI threads.
    /// </summary>
    public static void StartApiService()
    {
        Thread apiThread = new(() => _apiHost = StartAPI.StartApiServer());
        apiThread.IsBackground = true;
        apiThread.Start();
    }
}