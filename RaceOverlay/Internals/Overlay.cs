using System.Windows;

namespace RaceOverlay.Internals;

public abstract class Overlay: Window
{
     public virtual void _updateWindow(){}
     public virtual void _getData(){}

     public virtual void UpdateThreadMethod(){}
}