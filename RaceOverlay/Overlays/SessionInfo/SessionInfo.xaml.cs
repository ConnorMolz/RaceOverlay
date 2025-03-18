using System.Diagnostics;
using System.Windows;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.SessionInfo;

public partial class SessionInfo : Overlay
{
    private iRacingData _data;
    private double _timeLeft;
    private double _timeTotal;
    private int _lapsLeft;
    private int _lapsTotal;
    private int _lapsLeftEstimated;
    private int _maxIncidents;
    private int _incidents;
    
    public SessionInfo(): base("Session Info", "An overlay which shows the remaining race distance and current incidents.")
    {
        InitializeComponent();
        
        _setWindowSize(180, 60);
        
        Thread thread = new Thread(UpdateThreadMethod);
        
        thread.IsBackground = true;
        thread.Start();
    }

    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        _timeLeft = _data.SessionData.TimeLeft;
        _timeTotal = _data.SessionData.TimeTotal;
        _lapsLeft = _data.SessionData.LapsLeft;
        _lapsTotal = _data.SessionData.LapsTotal;
        _lapsLeftEstimated = _data.SessionData.LapsLeftEstimated;
        _maxIncidents = _data.SessionData.MaxIncidents;
        _incidents = _data.SessionData.Incidents;
        if (!_devMode)
        {
            InCar = _data.InCar;
        }
        else
        {
            InCar = true;
        }
    }

    public override void _updateWindow()
    {
        
        // 32767 is the default value for lapsTotal when the session is not using laps for the distance
        if (_lapsTotal == 32767)
        {
            // Time Formatting
            // 00:00:00 / 00:00:00 ~ 0/0 Laps
            TimeSpan timeLeft = TimeSpan.FromSeconds(_timeLeft);
            TimeSpan timeTotal = TimeSpan.FromSeconds(_timeTotal);
            TimeOrLaps.Text = $"{timeLeft:hh\\:mm\\:ss} / {timeTotal:hh\\:mm\\:ss}";
        }
        
        
        if (_lapsTotal != 32767)
        {
            // Lap Formatting
            // 0/0 Laps
            TimeOrLaps.Text = $"{_lapsTotal - _lapsLeft}/{_lapsTotal} Laps";
        }
        
        // Incident Formating
        IncidentsText.Text = $"{_incidents}/{_maxIncidents}";
    }

    public override void UpdateThreadMethod()
    {
        {
            while (true)
            {
                _getData();
                if (IsVisible)
                {
                    
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
    
    protected override void _scaleWindow(double scale)
    {
        try
        {
            ContentScaleTransform.ScaleX = scale;
            ContentScaleTransform.ScaleY = scale;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }
}