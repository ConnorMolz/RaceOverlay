using System.Windows;
using System.Windows.Input;

namespace RaceOverlay.Internals;

public abstract class Overlay: Window
{
     public String OverlayName { get; set; }
     public String OverlayDescription { get; set; }
     
     public virtual void _updateWindow(){}
     public virtual void _getData(){}

     public virtual void UpdateThreadMethod(){}

     public void ToggleOverlay()
     {
          if (IsVisible)
          {
               Hide();
               return;
          }
          Show();
     }
     
     public Overlay(String overlayName, String overlayDescription)
     {
          OverlayName = overlayName;
          OverlayDescription = overlayDescription;
          
          // Register the key down event handler
          this.KeyDown += Overlay_KeyDown;
     }
    
     private void Overlay_KeyDown(object sender, KeyEventArgs e)
     {
          // Check if F7 key was pressed
          if (e.Key == Key.F7)
          {
               Console.WriteLine($"F7 pressed in overlay: {OverlayName}");
               // You can add additional actions here
          }
     }
}