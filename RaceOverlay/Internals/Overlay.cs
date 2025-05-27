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

/// <summary>
/// Method for Overlays.
/// Following Method need to be implemented: <br/>
/// - _updateWindow() <see cref="_updateWindow"/> <br/>
/// - _getData() <see cref="_getData"/> <br/>
/// - _scaleWindow(double scale) <see cref="_scaleWindow"/> <br/>
/// </summary>
public abstract class Overlay: Window, INotifyPropertyChanged
{
     // Window size, scaling, opacity
     private int _windowWidth = 300;
     private int _windowHeight = 200;
     
     protected double _scale = 1;
     protected double _opacity = 1;
     
     // Controlling visibility and state
     protected bool _windowIsActive;
     protected bool _inCar = false;
     
     // Specific properties for the overlay in development cases
     protected bool _isTest = false;
     protected bool _devMode = false;
     
     // Variables for drag and drop functionality
     public Grid DragGrid { get; set; }
     private MouseButtonEventHandler _dragMoveHandler;
     
     /// <summary>
     /// Overlay name which is used to identify the overlay in settings and UI.
     /// </summary>
     public String OverlayName { get; set; }
     /// <summary>
     /// Description of the overlay, used for display purposes.
     /// </summary>
     public String OverlayDescription { get; set; }
     
     /// <summary>
     /// Status which is used to lock the position of the overlay and prevent it from being moved by the user.
     /// </summary>
     public bool PositionIsLocked { get; set; } = true;

     /// <summary>
     /// Method to update the overlay window.
     /// </summary>
     public abstract void _updateWindow();
     
     /// <summary>
     /// Method which will get the data for the overlay from the mapped iRacing Data <see cref="Data.Models.iRacingData"/>
     /// </summary>
     public abstract void _getData();
     
     /// <summary>
     /// Method which should be implemented to load the configuration for the overlay if needed.
     /// Scaling and opacity are handled by the base class and should not be loaded here.
     /// </summary>
     protected virtual void _getConfig(){}
     
     /// <summary>
     /// Method for Overlays.
     /// Following Method need to be implemented: <br/>
     /// - _updateWindow() <see cref="_updateWindow"/> <br/>
     /// - _getData() <see cref="_getData"/> <br/>
     /// - _scaleWindow(double scale) <see cref="_scaleWindow"/> <br/>
     /// </summary>
     /// <param name="overlayName">Overlay name which will identify the overlay</param>
     /// <param name="overlayDescription">Description to tell the user what the overlay will do</param>
     /// <param name="isTest">For Unit testing purpose to not loading Config from non existing file</param>
     public Overlay(String overlayName, String overlayDescription, bool? isTest = null)
     {
          AllowsTransparency = true;
          WindowStyle = WindowStyle.None;
          OverlayName = overlayName;
          OverlayDescription = overlayDescription;
          _isTest = isTest ?? false;


          // Register the key down event handler
          this.KeyDown += Overlay_KeyDown;
          if(!_isTest)
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

               if (settingsObject["Overlays"][OverlayName]["active"] == null)
               {
                    settingsObject["Overlays"][OverlayName]["active"] = false;
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
               if(settingsObject["Overlays"][OverlayName]["Top"] == null || settingsObject["Overlays"][OverlayName]["Left"] == null)
               {
                    settingsObject["Overlays"][OverlayName]["Top"] = 0;
                    settingsObject["Overlays"][OverlayName]["Left"] = 0;
                    File.WriteAllText(settingsFilePath, settingsObject.ToString());
               }
               Left = (int)settingsObject["Overlays"][OverlayName]["Left"];
               Top = (int)settingsObject["Overlays"][OverlayName]["Top"];
               _scale = _getDoubleConfig("_scale");
               if (_scale == 0 || _scale == null)
               {
                    _scale = 1;

                    _setDoubleConfig("_scale", 1);
               }
               
               _opacity = _getDoubleConfig("_opacity");
               if (_opacity == 0 || _opacity == null)
               {
                    _opacity = 1;

                    _setDoubleConfig("_opacity", 1);
               }

               ScaleValueChanges(_scale);
          }

     }   

     /// <summary>
     /// Method which will be run in a separate thread to update the overlay window.
     /// Edit this method if needed to change the update frequency or logic.
     /// Has no exit purposely, as it should run until the application is closed.
     /// </summary>
     public virtual void UpdateThreadMethod()
     {
          
          while (true)
          {
               try
               {
                    _getData();
                    if (IsVisible)
                    {

                         // Use Dispatcher to update UI from background thread
                         Dispatcher.Invoke(() => { _updateWindow(); });
                    }

                    // Add a small delay to prevent high CPU usage
                    Thread.Sleep(16); // ~60 updates per second
               }
               catch (Exception e)
               {
                    Debug.WriteLine(e);
               }
          }
          
     }
     

     /// <summary>
     /// Getting the Scale of the overlay window (defined in the config).
     /// </summary>
     /// <returns>Scale value</returns>
     public double getScale()
     {
          return _scale;
     }

     /// <summary>
     /// Set window size to binding values in the xaml file and not hardcoding values there.
     /// </summary>
     /// <param name="width">Window Width</param>
     /// <param name="height">Window Height</param>
     protected void _setWindowSize(int width, int height)
     {
          _windowWidth = width;
          _windowHeight = height;
          
          Width = _windowWidth;
          Height = _windowHeight;
     }

     /// <summary>
     /// Needs to be implemented in the derived class to scale the window.<br/>
     /// Example code:
     /// <code>
     /// try
     /// {
     ///    ContentScaleTransform.ScaleX = scale;
     ///    ContentScaleTransform.ScaleY = scale;
     /// }
     /// catch (Exception e)
     /// {
     ///    Debug.WriteLine(e);
     /// }
     /// </code>
     /// </summary>
     /// <param name="scale">Scale value is provided by <see cref="MainWindow.ScaleSlider_ValueChanged"/> or <see cref="MainWindow.ScaleInput_TextChanged"/></param>
     protected abstract void _scaleWindow(double scale);

     /// <summary>
     /// Method which should return a Grid with the configuration options for the overlay.<br/>
     /// Is shown after Overlay is selected in the MainWindow.<br/>
     /// </summary>
     public virtual Grid GetConfigs()
     {
          return new Grid();
     }

     
     /// <summary>
     /// Method to change the opacity of the window.<br/>
     /// Example code:
     /// </summary>
     /// <param name="newOpacity">Scale value is provided by <see cref="MainWindow.OpacitySlider"/> or <see cref="MainWindow.OpacityInput"/></param>
     public void OpacityValueChanges(double newOpacity)
     {
          _setDoubleConfig("_opacity", newOpacity);
          _setOpacity(newOpacity);
          if (IsVisible)
          {
               _setOpacity(newOpacity);
          }
     }
     
     /// <summary>
     /// Method to set the opacity of the window.<br/>
     /// Example code:
     /// </summary>
     /// <param name="newOpacity">Scale value is provided by <see cref="MainWindow.OpacitySlider"/> or <see cref="MainWindow.OpacityInput"/></param>
     private void _setOpacity(double newOpacity)
     {
          _opacity = newOpacity;
          Opacity = _opacity;
     }
     
     /// <summary>
     /// Method to change the scale of the window.<br/>
     /// </summary>
     public void ScaleValueChanges(double newScale)
     {
          _setDoubleConfig("_scale", newScale);
          _scaleWindowSize(newScale);
          if (IsVisible)
          {
               _scaleWindow(newScale);
          }
     }

     
     /// <summary>
     /// Mehtod to set the scale of the window.<br/>
     /// </summary>
     /// <param name="scale"></param>
     private void _scaleWindowSize(double scale)
     {
          Width = _windowWidth * scale;
          Height = _windowHeight * scale;
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

          settingsObject["Overlays"][OverlayName]["Configs"][key] = value;
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
          
          settingsObject["Overlays"][OverlayName]["Configs"][key] = value;
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
          
          settingsObject["Overlays"][OverlayName]["Configs"][key] = value;
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
          
          settingsObject["Overlays"][OverlayName]["Configs"][key] = value;
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
          
          settingsObject["Overlays"][OverlayName]["Configs"][key] = value;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
     }
     
     

     /// <summary>
     /// Method to toggle the visibility of the overlay and save the state of it.<br/>
     /// </summary>
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
    
     /// <summary>
     /// Method to handle the key down event for the overlay window.<br/>
     /// </summary>
     /// <param name="sender"></param>
     /// <param name="e"></param>
     private void Overlay_KeyDown(object sender, KeyEventArgs e)
     {
          // Check if F12 key was pressed
          if (e.Key == Key.F12)
          {
               Debug.WriteLine($"F12 pressed in overlay: {OverlayName}");
               TogglePositionLock();
          }
     }
     
     /// <summary>
     /// Mehtod to toggle the position lock of the overlay window and save it to the config.<br/>
     /// </summary>
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

     /// <summary>
     /// Method to handle the closing event of the overlay window.<br/>
     /// </summary>
     /// <param name="e"></param>
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

     /// <summary>
     /// Method to turn the application off by hiding the overlay window.<br/>
     /// </summary>
     public void TurnAppOff()
     {
          Hide();
     }
     
     /// <summary>
     /// Method to handle the content rendered event of the overlay window.<br/>
     /// </summary>
     /// <param name="e"></param>
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

     /// <summary>
     /// Method to show the overlay window on telemetry data.<br/>
     /// </summary>
     public void ShowOnTelemetry()
     {
          if (_windowIsActive)
          {
               Dispatcher.Invoke(() => { Show(); });
          }
     }
     
     /// <summary>
     ///  Method to hide the overlay window when telemetry data is not available or the window is closed.<br/>
     /// </summary>
     public void HideOnClosed()
     {
          if (_windowIsActive)
          {
               Dispatcher.Invoke(() => { Hide(); });
          }
     }
     
     /// <summary>
     /// Variable to track the inCar status of the overlay.<br/>
     /// </summary>
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

     /// <summary>
     /// Event handler for when the inCar status changes.<br/>
     /// </summary>
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
    
     /// <summary>
     /// Rerender Window when a property changes. Need to be called for this<br/>
     /// </summary>
     /// <param name="propertyName"></param>
     protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
     {
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
     }
     
     
}