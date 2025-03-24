using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json.Linq;
using RaceOverlay.Data;

namespace RaceOverlay.Internals;

public abstract class Overlay: Window, INotifyPropertyChanged
{
     private int _windowWidth = 300;
     private int _windowHeight = 200;
     protected bool _devMode = false;
     protected double _scale = 1;
     protected bool _windowIsActive;
     protected bool _inCar = false;
     protected bool _isTest = false;
     public String OverlayName { get; set; }
     public String OverlayDescription { get; set; }
     public bool PositionIsLocked { get; set; } = true;

     public abstract void _updateWindow();
     public abstract void _getData();
     protected virtual void _getConfig(){}

     public abstract void UpdateThreadMethod();
     public Grid DragGrid { get; set; }
     private MouseButtonEventHandler _dragMoveHandler;
     
     // Declare the event using EventHandler

     public Overlay(String overlayName, String overlayDescription, bool? isTest = false)
     {
          AllowsTransparency = true;
          WindowStyle = WindowStyle.None;
          OverlayName = overlayName;
          OverlayDescription = overlayDescription;


          // Register the key down event handler
          this.KeyDown += Overlay_KeyDown;
          if(_isTest)
          {
          // Set window position
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);

          if (settingsObject["Overlays"][OverlayName] == null)
          {
               settingsObject["Overlays"][OverlayName] = new JObject();
               settingsObject["Overlays"][OverlayName]["active"] = false;
               settingsObject["Overlays"][OverlayName]["Top"] = 0;
               settingsObject["Overlays"][OverlayName]["Left"] = 0;
               if (settingsObject["Overlays"][OverlayName]["Configs"] == null)
               {
                    settingsObject["Overlays"][OverlayName]["Configs"] = new JObject();
               }

               File.WriteAllText(settingsFilePath, settingsObject.ToString());
          }

          if (settingsObject["Overlays"][OverlayName]["Configs"] == null)
          {
               settingsObject["Overlays"][OverlayName]["Configs"] = new JObject();
               File.WriteAllText(settingsFilePath, settingsObject.ToString());
          }

          _windowIsActive = (bool)settingsObject["Overlays"][OverlayName]["active"];
          if (settingsObject["Dev"] == null)
          {
               _devMode = false;
          }
          else
          {
               _devMode = (bool)settingsObject["Dev"];
          }

          if (_windowIsActive && _devMode)
          {
               Show();
          }

          Left = (int)settingsObject["Overlays"][OverlayName]["Left"];
          Top = (int)settingsObject["Overlays"][OverlayName]["Top"];
          _scale = _getDoubleConfig("_scale");
          if (_scale == 0 || _scale == null)
          {
               _scale = 1;

               _setDoubleConfig("_scale", 1);
          }

          ScaleValueChanges(_scale);
     }

}

     public double getScale()
     {
          return _scale;
     }

     protected void _setWindowSize(int width, int height)
     {
          _windowWidth = width;
          _windowHeight = height;
          
          Width = _windowWidth;
          Height = _windowHeight;
     }

     protected abstract void _scaleWindow(double scale);

     public virtual Grid GetConfigs()
     {
          return new Grid();
     }

     protected virtual void _loadConfig(){}

     public void ScaleValueChanges(double newScale)
     {
          _setDoubleConfig("_scale", newScale);
          _scaleWindowSize(newScale);
          if (IsVisible)
          {
               _scaleWindow(newScale);
          }
     }

     private void _scaleWindowSize(double scale)
     {
          Width = _windowWidth * scale;
          Height = _windowHeight * scale;
     }
     
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

          if (settingsObject["Overlays"][OverlayName] == null)
          {
               settingsObject["Overlays"][OverlayName] = new JObject();
          }

          if (settingsObject["Overlays"][OverlayName]["Configs"] == null)
          {
               settingsObject["Overlays"][OverlayName]["Configs"] = new JObject();
          }

          if (settingsObject["Overlays"][OverlayName]["Configs"][key] != null)
          {
               return settingsObject["Overlays"][OverlayName]["Configs"][key].ToString();
          }

          settingsObject["Overlays"][OverlayName]["Configs"][key] = "";
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
          return "";
     }

     protected void _setStringConfig(string key, string value)
     {
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);

          settingsObject["Overlays"][OverlayName]["Configs"][key] = value;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
     }
     
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

          if (settingsObject["Overlays"][OverlayName] == null)
          {
               settingsObject["Overlays"][OverlayName] = new JObject();
          }

          if (settingsObject["Overlays"][OverlayName]["Configs"] == null)
          {
               settingsObject["Overlays"][OverlayName]["Configs"] = new JObject();
          }

          if (settingsObject["Overlays"][OverlayName]["Configs"][key] != null)
          {
               return (int)settingsObject["Overlays"][OverlayName]["Configs"][key];
          }

          settingsObject["Overlays"][OverlayName]["Configs"][key] = 0;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
          return 0;
     }

     protected void _setIntConfig(string key, int value)
     {
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);
          
          settingsObject["Overlays"][OverlayName]["Configs"][key] = value;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
     }

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

          if (settingsObject["Overlays"][OverlayName] == null)
          {
               settingsObject["Overlays"][OverlayName] = new JObject();
          }

          if (settingsObject["Overlays"][OverlayName]["Configs"] == null)
          {
               settingsObject["Overlays"][OverlayName]["Configs"] = new JObject();
          }

          if (settingsObject["Overlays"][OverlayName]["Configs"][key] != null)
          {
               return (float)settingsObject["Overlays"][OverlayName]["Configs"][key];
          }

          settingsObject["Overlays"][OverlayName]["Configs"][key] = 0;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
          return 0;
     }
     
     protected void _setFloatConfig(string key, float value)
     {
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);
          
          settingsObject["Overlays"][OverlayName]["Configs"][key] = value;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
     }
     
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

          if (settingsObject["Overlays"][OverlayName] == null)
          {
               settingsObject["Overlays"][OverlayName] = new JObject();
          }

          if (settingsObject["Overlays"][OverlayName]["Configs"] == null)
          {
               settingsObject["Overlays"][OverlayName]["Configs"] = new JObject();
          }

          if (settingsObject["Overlays"][OverlayName]["Configs"][key] != null)
          {
               return (double)settingsObject["Overlays"][OverlayName]["Configs"][key];
          }

          settingsObject["Overlays"][OverlayName]["Configs"][key] = 0;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
          return 0;
     }
     
     protected void _setDoubleConfig(string key, double value)
     {
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);
          
          settingsObject["Overlays"][OverlayName]["Configs"][key] = value;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
     }
     
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
    
          if (settingsObject["Overlays"][OverlayName] == null)
          {
               settingsObject["Overlays"][OverlayName] = new JObject();
          }
    
          if (settingsObject["Overlays"][OverlayName]["Configs"] == null)
          {
               settingsObject["Overlays"][OverlayName]["Configs"] = new JObject();
          }

          if (settingsObject["Overlays"][OverlayName]["Configs"][key] != null)
          {
               return (bool)settingsObject["Overlays"][OverlayName]["Configs"][key];
          }
    
          settingsObject["Overlays"][OverlayName]["Configs"][key] = false;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
          return false;
     }

     protected void _setBoolConfig(string key, bool value)
     {
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);
          
          settingsObject["Overlays"][OverlayName]["Configs"][key] = value;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
     }
     
     

     public void ToggleOverlay()
     {
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);

          // Ensure the required structure exists
          if (settingsObject["Overlays"] == null)
          {
               settingsObject["Overlays"] = new JObject();
          }

          if (settingsObject["Overlays"][OverlayName] == null)
          {
               settingsObject["Overlays"][OverlayName] = new JObject();
          }

          if (IsVisible)
          {
               Hide();
               settingsObject["Overlays"][OverlayName]["active"] = false;
          }
          else
          {
               Show();
               settingsObject["Overlays"][OverlayName]["active"] = true;
               _scaleWindow(this._scale);
          }

          File.WriteAllText(settingsFilePath, settingsObject.ToString());
     }
    
     private void Overlay_KeyDown(object sender, KeyEventArgs e)
     {
          // Check if F12 key was pressed
          if (e.Key == Key.F12)
          {
               Debug.WriteLine($"F12 pressed in overlay: {OverlayName}");
               TogglePositionLock();
          }
     }
     
     private void TogglePositionLock()
     {
          if(PositionIsLocked)
          {
               _dragMoveHandler = (sender, args) => DragMove();
               MouseLeftButtonDown += _dragMoveHandler;
               PositionIsLocked = false;
               return;
          }
          MouseLeftButtonDown -= _dragMoveHandler;
          PositionIsLocked = true;
          
          
          //write new position to settings.json
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);
          settingsObject["Overlays"][OverlayName]["Top"] = (int)Top;
          settingsObject["Overlays"][OverlayName]["Left"] = (int)Left;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
          
     }

     protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
     {
          e.Cancel = true;
          try
          {
               if (MainWindow.ShutdownIsTriggerd)
               {
                    TurnAppOff();
               }
               else
               {
                    ToggleOverlay();
               }
          }
          catch (InvalidOperationException ex)
          {
               Debug.WriteLine(ex.Message);
          }
     }

     public void TurnAppOff()
     {
          Hide();
     }
     
     protected override void OnContentRendered(EventArgs e)
     {
          base.OnContentRendered(e);
    
          // Apply scaling when the window is shown
          if (IsVisible)
          {
               _scaleWindow(_scale);
               _scaleWindowSize(_scale);
          }
     }

     public void ShowOnTelemetry()
     {
          if (_windowIsActive)
          {
               Dispatcher.Invoke(() => { Show(); });
          }
     }
     
     public void HideOnClosed()
     {
          if (_windowIsActive)
          {
               Dispatcher.Invoke(() => { Hide(); });
          }
     }
     
     // Property to track the _inCar status
     public bool InCar
     {
          get => _inCar;
          set
          {
               if (_inCar != value)
               {
                    _inCar = value;
                    OnPropertyChanged();
                    OnInCarChanged();
               }
          }
     }

     protected virtual void OnInCarChanged()
     {
          if (_inCar && _windowIsActive)
          {
               ShowOnTelemetry();
          }
          else
          {
               HideOnClosed();
          }
     }
     
     
     // Add INotifyPropertyChanged implementation
     public event PropertyChangedEventHandler? PropertyChanged;
    
     protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
     {
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
     }
     
     
}