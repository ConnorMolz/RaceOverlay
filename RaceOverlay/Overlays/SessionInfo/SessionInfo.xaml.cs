using System.Windows;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.SessionInfo;

public partial class SessionInfo : Overlay
{
    private iRacingData _data;
    private float _timeLeft;
    private float _timeTotal;
    private int _lapsLeft;
    private int _lapsTotal;
    private int _lapsLeftEstimated;
    private int _maxIncidents;
    private int _incidents;
    
    public SessionInfo(): base("Session Info", "TODO")
    {
        InitializeComponent();
        
        Thread thread = new Thread(UpdateThreadMethod);
        
        thread.IsBackground = true;
        thread.Start();
    }

    public override void _getData()
    {
        base._getData();
        _data = MainWindow.IRacingData;
        _timeLeft = _data.SessionData.TimeLeft;
        _timeTotal = _data.SessionData.TimeTotal;
        _lapsLeft = _data.SessionData.LapsLeft;
        _lapsTotal = _data.SessionData.LapsTotal;
        _lapsLeftEstimated = _data.SessionData.LapsLeftEstimated;
        _maxIncidents = _data.SessionData.MaxIncidents;
        _incidents = _data.SessionData.Incidents;
    }

    public override void _updateWindow()
    {
        base._updateWindow();
        
        // TODO: Time Formating
        
        // TODO: Lap Formating
        
        // Incident Formating
        IncidentsText.Text = _incidents + "/" + _maxIncidents;
    }

    public override void UpdateThreadMethod()
    {
        base.UpdateThreadMethod();
        {
            while (true)
            {
                if (IsVisible)
                {
                    _getData();
                    
                    // Use Dispatcher to update UI from background thread
                    Dispatcher.Invoke(() =>
                    {
                        _updateWindow();
                    });
                }
                
                // Add a small delay to prevent high CPU usage
                Thread.Sleep(16); // ~60 updates per second
            }
        }
    }
}