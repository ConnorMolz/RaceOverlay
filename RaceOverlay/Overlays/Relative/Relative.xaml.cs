using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;
using RaceOverlay.Internals.Configs;

namespace RaceOverlay.Overlays.Relative;

public partial class Relative : Overlay
{
    private iRacingData _data;
    private List<DriverModel> _driverModels;
    private int _additionalDrivers;
    private double _timeLeft;
    private double _timeTotal;
    private int _lapsLeft;
    private int _lapsTotal;
    private int _lapsLeftEstimated;
    private int _maxIncidents;
    private int _incidents;
    
    
    //TODO: Add Description
    public Relative() : base("Relative", "")
    {
        InitializeComponent();
        _getConfig();
        _setWindowSize(280, calcHeight());
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }
    
    public Relative(bool isTest) : base("Relative", "", isTest)
    {
        InitializeComponent();
        _getConfig();
        _setWindowSize(230, calcHeight());

        if (!_isTest)
        {
            Thread updateThread = new Thread(UpdateThreadMethod);

            updateThread.IsBackground = true;
            updateThread.Start();
        }
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
        
        // SOF Formating
        SOFText.Text = $"SOF: {_data.SessionData.SOF}";

        try
        {
            Body.Children.Clear();
            for (int i = 0; i < _driverModels.Count; i++)
            {
                Body.RowDefinitions.Add(new RowDefinition());
                var row = new RelativeRow(
                    _driverModels[i].Name,
                    _driverModels[i].Position,
                    Math.Abs(_data.GetGapToPlayerMs(_driverModels[i].Idx)),
                    _driverModels[i].CarNumber,
                    _driverModels[i].ClassColorCode
                );
                Grid.SetRow(row, i);
                
                Body.Children.Add(row);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
       
    }

    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        if (_data == null)
            return;
        _driverModels = GetRelative();
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

    private List<DriverModel> GetRelative()
    {
        var relative = new List<DriverModel>();
        var drivers = _data.Drivers.ToList();
        if (drivers == null || drivers.Count == 0)
            return relative;
        
        // calculate live standings based on percentage of the current lap completed
        drivers.Sort((a, b) =>
        {

            if (a == null) return -1;
            if (b == null) return 1;

            var aSpline = a.LapDistance;
            var bSpline = b.LapDistance;

            float aPosition = aSpline / 10;
            float bPosition = bSpline / 10;
            return aPosition.CompareTo(bPosition);
        });

        int playerEntryListIndex = -1;
        int sortedListIndex = 0;
        foreach (DriverModel car in drivers)
        {
            if (_data.PlayerIdx == car.Idx)
            {
                playerEntryListIndex = sortedListIndex;
                break;
            }
            sortedListIndex++;
        }
        
        
        // Collect "additionalDrivers" in front of player and after. Limit to max 40secs difference.
        int startIndex = (playerEntryListIndex - _additionalDrivers + drivers.Count) % drivers.Count;
        int endIndex = (playerEntryListIndex + _additionalDrivers + 1 + drivers.Count) % drivers.Count;
        
        for (int index = startIndex; index < endIndex; index++)
        {

            DriverModel carToAdd = drivers[index];

            if (Math.Abs(_data.GetGapToPlayerMs(carToAdd.Idx)) > 40000) continue;
            relative.Insert(0, carToAdd);
        }
        
        return relative;
    }
    

    protected override void _getConfig()
    {
        _additionalDrivers = _getIntConfig("_additionalRows");
    }

    public override Grid GetConfigs()
    {
        Grid grid = new Grid();
        
        grid.RowDefinitions.Add(new RowDefinition());
        
        InputElement additionalDrivers = new InputElement("AdditionalRows",  _additionalDrivers.ToString());
        additionalDrivers.SetValue(Grid.RowProperty, 0);
        
        void ParseAdditionalDriversInput(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(additionalDrivers.InputField.Text, out int maxValue))
            {
                _additionalDrivers = maxValue;
                _setIntConfig("_additionalRows", _additionalDrivers);
                _setWindowSize(230, calcHeight());
                _scaleWindow(_scale);
            }
        }
        
        additionalDrivers.InputField.TextChanged += ParseAdditionalDriversInput;
        
        grid.Children.Add(additionalDrivers);
        
        return grid;
    }

    private int calcHeight()
    {
        int height = 55;
        height += (30 * _additionalDrivers * 2);
        return height;
    }
    
    
}