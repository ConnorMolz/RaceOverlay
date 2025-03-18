using System.Diagnostics;
using System.Windows;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.Leaderboard;

public partial class Leaderboard : Overlay
{
    private iRacingData _data;

    private List<DriverModel> _drivers;
    private int _playerCarIdx;
    private double _timeLeft;
    private double _timeTotal;
    private int _lapsLeft;
    private int _lapsTotal;
    private int _lapsLeftEstimated;
    private float _trackTemp;
    private float _airTemp;
    private int _maxIncidents;
    private int _incidents;
    
    // TODO: Add Description
    public Leaderboard(): base("Leaderboard", "TODO")
    {
        InitializeComponent();
        _setWindowSize(300, 250);
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }

    public override void _updateWindow()
    {
        // Update the Header
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
        IncidentsText.Text = $"X: {_incidents}/{_maxIncidents}";
        
        AirTempText.Text = "Air: " + _airTemp.ToString("F1") + "C°";
        TrackTempText.Text = "Track: " +  _trackTemp.ToString("F1") + "C°";

        try
        {
            Body.Children.Clear();
            DriverModel player = _drivers.Find(driver => driver.Idx == _playerCarIdx);

            for (int i = player.Position - 2; i <= player.Position + 2; i++)
            {
                DriverModel driver = _drivers.Find(driver => driver.Position == i);
                if (driver.Idx == _playerCarIdx)
                {
                    LeaderBoardRow playerRow = new LeaderBoardRow(driver.Name, driver.Position, driver.LastLap,
                        driver.BestLap,
                        driver.iRating, driver.ClassColorCode);
                    playerRow.SetToPlayerRow();
                    Body.Children.Add(playerRow);
                }
                else
                {
                    Body.Children.Add(new LeaderBoardRow(driver.Name, driver.Position, driver.LastLap, driver.BestLap,
                        driver.iRating, driver.ClassColorCode));
                }

            }
        }
        catch (Exception e)
        {
            //ignore
        }

    }

    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        _drivers = _data.Drivers.ToList();
        _playerCarIdx = _data.PlayerIdx;
        _timeLeft = _data.SessionData.TimeLeft;
        _timeTotal = _data.SessionData.TimeTotal;
        _lapsLeft = _data.SessionData.LapsLeft;
        _lapsTotal = _data.SessionData.LapsTotal;
        _lapsLeftEstimated = _data.SessionData.LapsLeftEstimated;
        _maxIncidents = _data.SessionData.MaxIncidents;
        _incidents = _data.SessionData.Incidents;
        _airTemp = _data.WeatherData.AirTemp;
        _trackTemp = _data.WeatherData.TrackTemp;
        if (!_devMode)
        {
            InCar = _data.InCar;
        }
        else
        {
            InCar = true;
        }
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