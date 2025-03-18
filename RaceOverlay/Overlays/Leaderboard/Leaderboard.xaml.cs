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
    
    // TODO: Add Description
    public Leaderboard(): base("Leaderboard", "TODO")
    {
        InitializeComponent();
        _setWindowSize(200, 350);
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }

    public override void _updateWindow()
    {
        DriverModel player = _drivers.Find(driver => driver.Idx == _playerCarIdx);

        for (int i = player.Position - 2; i <= player.Position + 2; i++)
        {
            DriverModel driver = _drivers.Find(driver => driver.Position == i);
            if (driver.Idx == _playerCarIdx)
            {
                LeaderBoardRow playerRow = new LeaderBoardRow(driver.Name, driver.Position, driver.LastLap, driver.BestLap,
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

    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        _drivers = _data.Drivers.ToList();
        _playerCarIdx = _data.PlayerIdx;
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