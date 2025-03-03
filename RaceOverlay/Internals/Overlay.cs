using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json.Linq;

namespace RaceOverlay.Internals;

public abstract class Overlay: Window
{
     public String OverlayName { get; set; }
     public String OverlayDescription { get; set; }
     public bool PositionIsLocked { get; set; } = true;
     
     public virtual void _updateWindow(){}
     public virtual void _getData(){}

     public virtual void UpdateThreadMethod(){}

     public void ToggleOverlay()
     {
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);
          if (IsVisible)
          {
               Hide();
               settingsObject["Overlays"][OverlayName]["active"] = false;
               File.WriteAllText(settingsFilePath, settingsObject.ToString());
               return;
          }
          settingsObject["Overlays"][OverlayName]["active"] = true;
          File.WriteAllText(settingsFilePath, settingsObject.ToString());
          Show();
     }
     
     public Overlay(String overlayName, String overlayDescription)
     {
          OverlayName = overlayName;
          OverlayDescription = overlayDescription;
          
          // Register the key down event handler
          this.KeyDown += Overlay_KeyDown;
          
          // Set window position
          string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
          string jsonContent = File.ReadAllText(settingsFilePath);
          JObject settingsObject = JObject.Parse(jsonContent);
          
          if(settingsObject["Overlays"][OverlayName] == null)
          {
               settingsObject["Overlays"][OverlayName] = new JObject();
               settingsObject["Overlays"][OverlayName]["active"] = false;
               settingsObject["Overlays"][OverlayName]["Top"] = 0;
               settingsObject["Overlays"][OverlayName]["Left"] = 0;
               File.WriteAllText(settingsFilePath, settingsObject.ToString());
          }
          
          Left = (int)settingsObject["Overlays"][OverlayName]["Left"];
          Top = (int)settingsObject["Overlays"][OverlayName]["Top"];
          if((bool)settingsObject["Overlays"][OverlayName]["active"])
          {
               Show();
          }
     }
    
     private void Overlay_KeyDown(object sender, KeyEventArgs e)
     {
          // Check if F7 key was pressed
          if (e.Key == Key.F7)
          {
               Debug.WriteLine($"F7 pressed in overlay: {OverlayName}");
               TogglePositionLock();
          }
     }
     
     private void TogglePositionLock()
     {
          if(PositionIsLocked)
          {
               WindowStyle = WindowStyle.SingleBorderWindow;
               PositionIsLocked = false;
               return;
          }
          WindowStyle = WindowStyle.None;
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
               ToggleOverlay();
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
}