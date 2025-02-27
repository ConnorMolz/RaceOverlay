using System.Windows;

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
}