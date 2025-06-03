using System.IO;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;

namespace RaceOverlay.Internals;

/// <summary>
/// StreamOverlay is only for UI Reasons currently used.
/// </summary>
public abstract class StreamOverlay
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Link { get; set; }
    
    /// <summary>
    /// StreamOverlay is only for UI Reasons currently used.
    /// </summary>
    /// <param name="title">Title for UI.</param>
    /// <param name="description">Description for UI.</param>
    /// <param name="link">Link which the User will copy for use the stream overlay in streaming software like OBS</param>
    public StreamOverlay(string title, string description, string link)
    {
        Title = title;
        Description = description;
        Link = link;
    }

    /// <summary>
    /// Method to get the configuration values of the overlay.
    /// </summary>
    /// All values need to be set as public static properties in the derived class to get those in the Objects.
    protected virtual void _getConfig()
    {
        
    }
    
    /// <summary>
    /// Method to get the configuration of the overlay for showing in the MainWindow.
    /// </summary>
    /// Please override this method the virtual method returns an empty Grid.
    /// <returns>Grid which will show when selected in the Main Window.</returns>
    public virtual Grid GetConfig()
    {
        return new Grid();
    }
    
    
    /// <summary>
     /// Mehtod to get a string configuration value from the settings.json file.<br/>
     /// </summary>
     /// <param name="key">Json key which will be used to save and load the config value</param>
     /// <returns>String which is saved by the user or an empty string</returns>
     protected string _getStringConfig(string key)
     {
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);

          // Ensure the required structure exists
          if (settingsObject["Overlays"] == null)
          {
               settingsObject["Overlays"] = new JObject();
          }

          if (settingsObject["Overlays"][Title + "StreamOverlay"] == null)
          {
               settingsObject["Overlays"][Title + "StreamOverlay"] = new JObject();
          }

          if (settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"] == null)
          {
               settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"] = new JObject();
          }

          if (settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key] != null)
          {
               return settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key].ToString();
          }

          settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key] = "";
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
          return "";
     }

     /// <summary>
     /// Method to set a string configuration value in the settings.json file.<br/>
     /// </summary>
     /// <param name="key">Json key which will be used to safe and load the config value</param>
     /// <param name="value">Value which will set into the config</param>
     protected void _setStringConfig(string key, string value)
     {
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);

          settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key] = value;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
     }
     
     /// <summary>
     /// Method to get an integer configuration value from the settings.json file.<br/>
     /// </summary>
     /// <param name="key">Json key which will be used to save and load the config value</param>
     /// <returns>Int which is saved by the user or 0</returns>
     protected int _getIntConfig(string key)
     {
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);

          // Ensure the required structure exists
          if (settingsObject["Overlays"] == null)
          {
               settingsObject["Overlays"] = new JObject();
          }

          if (settingsObject["Overlays"][Title + "StreamOverlay"] == null)
          {
               settingsObject["Overlays"][Title + "StreamOverlay"] = new JObject();
          }

          if (settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"] == null)
          {
               settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"] = new JObject();
          }

          if (settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key] != null)
          {
               return (int)settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key];
          }

          settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key] = 0;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
          return 0;
     }

     /// <summary>
     /// Method to set an integer configuration value in the settings.json file.<br/>
     /// </summary>
     /// <param name="key">Json key which will be used to safe and load the config value</param>
     /// <param name="value">Value which will set into the config</param>
     protected void _setIntConfig(string key, int value)
     {
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);
          
          settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key] = value;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
     }

     /// <summary>
     /// Method to get a float configuration value from the settings.json file.<br/>
     /// </summary>
     /// <param name="key">Json key which will be used to save and load the config value</param>
     /// <returns>float which is saved by the user or 0</returns>
     protected float _getFloatConfig(string key)
     {
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);

          // Ensure the required structure exists
          if (settingsObject["Overlays"] == null)
          {
               settingsObject["Overlays"] = new JObject();
          }

          if (settingsObject["Overlays"][Title + "StreamOverlay"] == null)
          {
               settingsObject["Overlays"][Title + "StreamOverlay"] = new JObject();
          }

          if (settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"] == null)
          {
               settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"] = new JObject();
          }

          if (settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key] != null)
          {
               return (float)settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key];
          }

          settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key] = 0;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
          return 0;
     }
     
     /// <summary>
     /// Method to set a float configuration value in the settings.json file.<br/>
     /// </summary>
     /// <param name="key">Json key which will be used to safe and load the config value</param>
     /// <param name="value">Value which will set into the config</param>
     protected void _setFloatConfig(string key, float value)
     {
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);
          
          settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key] = value;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
     }
     
     /// <summary>
     /// Method to get a double configuration value from the settings.json file.<br/>
     /// </summary>
     /// <param name="key">Json key which will be used to save and load the config value</param>
     /// <returns>Double which is saved by the user or 0</returns>
     protected double _getDoubleConfig(string key)
     {
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);

          // Ensure the required structure exists
          if (settingsObject["Overlays"] == null)
          {
               settingsObject["Overlays"] = new JObject();
          }

          if (settingsObject["Overlays"][Title + "StreamOverlay"] == null)
          {
               settingsObject["Overlays"][Title + "StreamOverlay"] = new JObject();
          }

          if (settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"] == null)
          {
               settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"] = new JObject();
          }

          if (settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key] != null)
          {
               return (double)settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key];
          }

          settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key] = 0;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
          return 0;
     }
     
     /// <summary>
     /// Method to set a double configuration value in the settings.json file.<br/>
     /// </summary>
     /// <param name="key">Json key which will be used to safe and load the config value</param>
     /// <param name="value">Value which will set into the config</param>
     protected void _setDoubleConfig(string key, double value)
     {
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);
          
          settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key] = value;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
     }
     
     /// <summary>
     /// Method to get a boolean configuration value from the settings.json file.<br/>
     /// </summary>
     /// <param name="key">Json key which will be used to save and load the config value</param>
     /// <returns>Bool which is saved by the user or false</returns>
     protected bool _getBoolConfig(string key)
     {
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);

          // Ensure the required structure exists
          if (settingsObject["Overlays"] == null)
          {
               settingsObject["Overlays"] = new JObject();
          }
    
          if (settingsObject["Overlays"][Title + "StreamOverlay"] == null)
          {
               settingsObject["Overlays"][Title + "StreamOverlay"] = new JObject();
          }
    
          if (settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"] == null)
          {
               settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"] = new JObject();
          }

          if (settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key] != null)
          {
               return (bool)settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key];
          }
    
          settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key] = false;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
          return false;
     }

     /// <summary>
     /// Method to set a boolean configuration value in the settings.json file.<br/>
     /// </summary>
     /// <param name="key">Json key which will be used to safe and load the config value</param>
     /// <param name="value">Value which will set into the config</param>
     protected void _setBoolConfig(string key, bool value)
     {
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);
          
          settingsObject["Overlays"][Title + "StreamOverlay"]["Configs"][key] = value;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
     }
    
}