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
    
    // Control variables for header config
    private bool _showSessionTypeHeader;
    private bool _showRaceDistanceHeader;
    private bool _showAirTempHeader;
    private bool _showTrackTempHeader;
    private bool _showIncidentsHeader;
    private bool _showSOFHeader;
    private bool _showFuelHeader;
    private bool _showIsWetHeader;
    private bool _showSimTimeHeader;
    
    // Values for Header
    private double _timeLeft;
    private double _timeTotal;
    private int _lapsLeft;
    private int _lapsTotal;
    private int _lapsLeftEstimated;
    private int _maxIncidents;
    private int _incidents;
    private string _sessionType;
    private double _airTemp;
    private double _trackTemp;
    private bool _isWet;
    private int _sof;
    private float _fuel;
    private float _inSimTime;
    
    
    public Relative() : base("Relative", "Shows the Relative time to other cars inbound of 40 seconds.")
    {
        InitializeComponent();
        _getConfig();
        _updateHeader();
        _setWindowSize(336, calcHeight());
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }
    
    public Relative(bool isTest) : base("Relative", "", isTest)
    {
        InitializeComponent();
        _getConfig();
        _setWindowSize(336, calcHeight());

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
        SessionTypeHeaderText.Text = _sessionType;
        // 32767 is the default value for lapsTotal when the session is not using laps for the distance
        if (_lapsTotal == 32767)
        {
            // Time Formatting
            // 00:00:00 / 00:00:00 ~ 0/0 Laps
            TimeSpan timeLeft = TimeSpan.FromSeconds(_timeLeft);
            TimeSpan timeTotal = TimeSpan.FromSeconds(_timeTotal);
            TimeOrLapsHeaderText.Text = $"{timeLeft:hh\\:mm\\:ss} / {timeTotal:hh\\:mm\\:ss}";
        }
        
        
        if (_lapsTotal != 32767)
        {
            // Lap Formatting
            // 0/0 Laps
            TimeOrLapsHeaderText.Text = $"{_lapsTotal - _lapsLeft}/{_lapsTotal} Laps";
        }
        
        // Incident Formating
        IncidentsHeaderText.Text = $"X: {_incidents}/{_maxIncidents}";
        
        // SOF Formating
        SOFHeaderText.Text = $"SOF: {_sof}";
        
        // Fuel Formating
        FuelHeaderText.Text = $"Fuel: {_fuel:F1}L";
        
        // Is Wet Formating
        IsWetHeaderText.Text = _isWet ? "IS WET: YES" : "IS WET: NO";

        // Air Temp Formating
        AirTempHeaderText.Text = $"Air Temp: {_airTemp:F1}C°";
        
        // Track Temp Formating
        TrackTempHeaderText.Text = $"Track Temp: {_trackTemp:F1}C°";
        
        // Sim Time Formating
        TimeSpan simTime = TimeSpan.FromSeconds(_inSimTime);
        InSimTimeHeaderText.Text = $"{simTime:hh\\:mm}";
        
        try
        {
            var driver = _data.Drivers.ToList().Find(d => d.Idx == _data.PlayerIdx);
            Body.Children.Clear();
            for (int i = 0; i < _driverModels.Count; i++)
            {
                Body.RowDefinitions.Add(new RowDefinition());
                var row = new RelativeRow(
                    _driverModels[i].Name,
                    _driverModels[i].ClassPosition,
                    _data.GetGapToPlayerMs(driver, _driverModels[i]),
                    _driverModels[i].CarNumber,
                    MainWindow.IrsdkSharper.Data.SessionInfo.DriverInfo.Drivers.ElementAt(i).CarClassColor,
                    _driverModels[i].License);
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
        if (!_devMode)
        {
            InCar = _data.InCar;
        }
        else
        {
            InCar = true;
        }
        if (_data == null)
            return;
        _driverModels = FindCyclicalNeighbors(_data.Drivers.ToList().Find(d => d.Idx == _data.PlayerIdx), _data.Drivers.ToList());
        _timeLeft = _data.SessionData.TimeLeft;
        _timeTotal = _data.SessionData.TimeTotal;
        _lapsLeft = _data.SessionData.LapsLeft;
        _lapsTotal = _data.SessionData.LapsTotal;
        _lapsLeftEstimated = _data.SessionData.LapsLeftEstimated;
        _maxIncidents = _data.SessionData.MaxIncidents;
        _incidents = _data.SessionData.Incidents;
        _sessionType = _data.SessionData.SessionType;
        _airTemp = _data.WeatherData.AirTemp;
        _trackTemp = _data.WeatherData.TrackTemp;
        _isWet = _data.WeatherData.WeatherDeclaredWet;
        _sof = _data.SessionData.SOF;
        _fuel = _data.LocalCarTelemetry.FuelLevel;
        _inSimTime = _data.SessionData.InSimTime;

        
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


    protected override void _getConfig()
    {
        _additionalDrivers = _getIntConfig("_additionalRows");
        _showSessionTypeHeader = _getBoolConfig("_showSessionTypeHeader");
        _showRaceDistanceHeader = _getBoolConfig("_showRaceDistanceHeader");
        _showAirTempHeader = _getBoolConfig("_showAirTempHeader");
        _showTrackTempHeader = _getBoolConfig("_showTrackTempHeader");
        _showIncidentsHeader = _getBoolConfig("_showIncidentsHeader");
        _showSOFHeader = _getBoolConfig("_showSOFHeader");
        _showFuelHeader = _getBoolConfig("_showFuelHeader");
        _showIsWetHeader = _getBoolConfig("_showIsWetHeader");
        _showSimTimeHeader = _getBoolConfig("_showSimTimeHeader");
    }

    public override Grid GetConfigs()
    {
        Grid grid = new Grid();
        
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        
        
        CheckBoxElement showSessionTypeHeader = new CheckBoxElement("Show Session Type", _showSessionTypeHeader);
        showSessionTypeHeader.CheckBox.Checked += (sender, args) =>
        {
            _showSessionTypeHeader = true;
            _setBoolConfig("_showSessionTypeHeader", true);
            _updateHeader();
        };
        showSessionTypeHeader.CheckBox.Unchecked += (sender, args) =>
        {
            _showSessionTypeHeader = false;
            _setBoolConfig("_showSessionTypeHeader", false);
            _updateHeader();
        };
        
        showSessionTypeHeader.SetValue(Grid.RowProperty, 1);
        showSessionTypeHeader.SetValue(Grid.ColumnProperty, 0);
        grid.Children.Add(showSessionTypeHeader);
        
        
        CheckBoxElement showRaceDistanceHeader = new CheckBoxElement("Show Race Distance", _showRaceDistanceHeader);
        showRaceDistanceHeader.CheckBox.Checked += (sender, args) =>
        {
            _showRaceDistanceHeader = true;
            _setBoolConfig("_showRaceDistanceHeader", true);
            _updateHeader();
        };
        showRaceDistanceHeader.CheckBox.Unchecked += (sender, args) =>
        {
            _showRaceDistanceHeader = false;
            _setBoolConfig("_showRaceDistanceHeader", false);
            _updateHeader();
        };
        
        showRaceDistanceHeader.SetValue(Grid.RowProperty, 1);
        showRaceDistanceHeader.SetValue(Grid.ColumnProperty, 1);
        grid.Children.Add(showRaceDistanceHeader);
        
        CheckBoxElement showAirTempHeader = new CheckBoxElement("Show Air Temp", _showAirTempHeader);
        showAirTempHeader.CheckBox.Checked += (sender, args) =>
        {
            _showAirTempHeader = true;
            _setBoolConfig("_showAirTempHeader", true);
            _updateHeader();
        };
        showAirTempHeader.CheckBox.Unchecked += (sender, args) =>
        {
            _showAirTempHeader = false;
            _setBoolConfig("_showAirTempHeader", false);
            _updateHeader();
        };
        
        showAirTempHeader.SetValue(Grid.RowProperty, 2);
        showAirTempHeader.SetValue(Grid.ColumnProperty, 0);
        grid.Children.Add(showAirTempHeader);
        
        CheckBoxElement showTrackTempHeader = new CheckBoxElement("Show Track Temp", _showTrackTempHeader);
        showTrackTempHeader.CheckBox.Checked += (sender, args) =>
        {
            _showTrackTempHeader = true;
            _setBoolConfig("_showTrackTempHeader", true);
            _updateHeader();
        };
        showTrackTempHeader.CheckBox.Unchecked += (sender, args) =>
        {
            _showTrackTempHeader = false;
            _setBoolConfig("_showTrackTempHeader", false);
            _updateHeader();
        };
        
        showTrackTempHeader.SetValue(Grid.RowProperty, 2);
        showTrackTempHeader.SetValue(Grid.ColumnProperty, 1);
        grid.Children.Add(showTrackTempHeader);
        
        CheckBoxElement showIncidentsHeader = new CheckBoxElement("Show Incidents", _showIncidentsHeader);
        showIncidentsHeader.CheckBox.Checked += (sender, args) =>
        {
            _showIncidentsHeader = true;
            _setBoolConfig("_showIncidentsHeader", true);
            _updateHeader();
        };
        showIncidentsHeader.CheckBox.Unchecked += (sender, args) =>
        {
            _showIncidentsHeader = false;
            _setBoolConfig("_showIncidentsHeader", false);
            _updateHeader();
        };
        
        showIncidentsHeader.SetValue(Grid.RowProperty, 3);
        showIncidentsHeader.SetValue(Grid.ColumnProperty, 0);
        grid.Children.Add(showIncidentsHeader);
        
        CheckBoxElement showSOFHeader = new CheckBoxElement("Show SOF", _showSOFHeader);
        showSOFHeader.CheckBox.Checked += (sender, args) =>
        {
            _showSOFHeader = true;
            _setBoolConfig("_showSOFHeader", true);
            _updateHeader();
        };
        showSOFHeader.CheckBox.Unchecked += (sender, args) =>
        {
            _showSOFHeader = false;
            _setBoolConfig("_showSOFHeader", false);
            _updateHeader();
        };
        
        showSOFHeader.SetValue(Grid.RowProperty, 3);
        showSOFHeader.SetValue(Grid.ColumnProperty, 1);
        grid.Children.Add(showSOFHeader);

        CheckBoxElement showFuelHeader = new CheckBoxElement("Show Fuellevel", _showFuelHeader);
        showFuelHeader.CheckBox.Checked += (sender, args) =>
        {
            _showFuelHeader = true;
            _setBoolConfig("_showFuelHeader", true);
            _updateHeader();
        };
        showFuelHeader.CheckBox.Unchecked += (sender, args) =>
        {
            _showFuelHeader = false;
            _setBoolConfig("_showFuelHeader", false);
            _updateHeader();
        };
        
        showFuelHeader.SetValue(Grid.RowProperty, 4);
        showFuelHeader.SetValue(Grid.ColumnProperty, 0);
        grid.Children.Add(showFuelHeader);
        
        CheckBoxElement showIsWetHeader = new CheckBoxElement("Show Is Wet", _showIsWetHeader);
        showIsWetHeader.CheckBox.Checked += (sender, args) =>
        {
            _showIsWetHeader = true;
            _setBoolConfig("_showIsWetHeader", true);
            _updateHeader();
        };
        showIsWetHeader.CheckBox.Unchecked += (sender, args) =>
        {
            _showIsWetHeader = false;
            _setBoolConfig("_showIsWetHeader", false);
            _updateHeader();
        };
        
        showIsWetHeader.SetValue(Grid.RowProperty, 4);
        showIsWetHeader.SetValue(Grid.ColumnProperty, 1);
        grid.Children.Add(showIsWetHeader);

        CheckBoxElement showSimTimeHeader = new CheckBoxElement("Show in sim Time", _showSimTimeHeader);
        showSimTimeHeader.CheckBox.Checked += (sender, args) =>
        {
            _showSimTimeHeader = true;
            _setBoolConfig("_showSimTimeHeader", true);
            _updateHeader();
        };
        showSimTimeHeader.CheckBox.Unchecked += (sender, args) =>
        {
            _showSimTimeHeader = false;
            _setBoolConfig("_showSimTimeHeader", false);
            _updateHeader();
        };
        
        showSimTimeHeader.SetValue(Grid.RowProperty, 5);
        showSimTimeHeader.SetValue(Grid.ColumnProperty, 0);
        grid.Children.Add(showSimTimeHeader);
        
        return grid;
    }

    private int calcHeight()
    {
        int height = 45;
        height += (20 * _additionalDrivers * 2);
        return height;
    }

    private void _updateHeader()
    {
        SessionTypeHeaderText.Visibility = Visibility.Collapsed;
        TimeOrLapsHeaderText.Visibility = Visibility.Collapsed;
        AirTempHeaderText.Visibility = Visibility.Collapsed;
        TrackTempHeaderText.Visibility = Visibility.Collapsed;
        IncidentsHeaderText.Visibility = Visibility.Collapsed;
        SOFHeaderText.Visibility = Visibility.Collapsed;
        FuelHeaderText.Visibility = Visibility.Collapsed;
        IsWetHeaderText.Visibility = Visibility.Collapsed;
        InSimTimeHeaderText.Visibility = Visibility.Collapsed;

        if (_showSessionTypeHeader)
        {
            SessionTypeHeaderText.Visibility = Visibility.Visible;
        }
        if (_showRaceDistanceHeader)
        {
            TimeOrLapsHeaderText.Visibility = Visibility.Visible;
        }
        if (_showAirTempHeader)
        {
            AirTempHeaderText.Visibility = Visibility.Visible;
        }
        if (_showTrackTempHeader)
        {
            TrackTempHeaderText.Visibility = Visibility.Visible;
        }
        if (_showIncidentsHeader)
        {
            IncidentsHeaderText.Visibility = Visibility.Visible;
        }
        if (_showSOFHeader)
        {
            SOFHeaderText.Visibility = Visibility.Visible;
        }
        if (_showFuelHeader)
        {
            FuelHeaderText.Visibility = Visibility.Visible;
        }
        if (_showIsWetHeader)
        {
            IsWetHeaderText.Visibility = Visibility.Visible;
        }
        if (_showSimTimeHeader)
        {
            InSimTimeHeaderText.Visibility = Visibility.Visible;
        }

    }
    
     public List<DriverModel>  FindCyclicalNeighbors(
        DriverModel myObject,
        List<DriverModel> allObjects)
    {
        if (myObject == null) throw new ArgumentNullException(nameof(myObject));
        if (allObjects == null) throw new ArgumentNullException(nameof(allObjects));

        double myVal = myObject.LapPtc;

        // Using ValueTuples to store candidates with their distances
        var aheadCandidates = new List<(DriverModel Object, double Distance)>();
        var behindCandidates = new List<(DriverModel Object, double Distance)>();

        foreach (var obj in allObjects)
        {
            // Skip the object itself using reference equality
            if (ReferenceEquals(obj, myObject))
            {
                continue;
            }

            double otherVal = obj.LapPtc;

            // Calculate distance in the "ahead" direction (myVal -> otherVal, positive wrap)
            // C# % operator: (a % n) can be negative if a is negative.
            // (a % n + n) % n ensures a positive result.
            // Here, (otherVal - myVal) can range from approx -100 to 100.
            // So, (otherVal - myVal + 100.0) ranges from approx 0 to 200.
            // Thus, (otherVal - myVal + 100.0) % 100.0 will correctly yield [0, 100).
            double distAhead = (otherVal - myVal + 1.00) % 1.00;
            aheadCandidates.Add((obj, distAhead));

            // Calculate distance in the "behind" direction (myVal -> otherVal, negative wrap)
            double distBehind = (myVal - otherVal + 1.00) % 1.00;
            behindCandidates.Add((obj, distBehind));
        }

        // Filter out objects at the exact same percentage point (Distance == 0.0),
        // then sort by distance and take up to the top 2.
        // "Ahead" or "Behind" implies a different location.
        List<DriverModel> finalAhead = aheadCandidates
            .Where(c => c.Distance > 0.0) // Exclude objects at the exact same percentage
            .OrderBy(c => c.Distance)
            .Select(c => c.Object)
            .Take(_additionalDrivers)
            .ToList();

        List<DriverModel> finalBehind = behindCandidates
            .Where(c => c.Distance > 0.0) // Exclude objects at the exact same percentage
            .OrderBy(c => c.Distance)
            .Select(c => c.Object)
            .Take(_additionalDrivers)
            .ToList();
        List<DriverModel> result = new List<DriverModel>(); 
        finalAhead.Reverse();
        foreach (var driver in finalAhead)
        {
            result.Add(driver);
        }
        result.Add(myObject);
        foreach (var driver in finalBehind)
        {
            result.Add(driver);
        }

        return result;
    }
    
    
     
}