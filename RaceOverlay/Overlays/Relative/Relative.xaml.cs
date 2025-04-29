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
            Body.Children.Clear();
            for (int i = 0; i < _driverModels.Count; i++)
            {
                Body.RowDefinitions.Add(new RowDefinition());
                var row = new RelativeRow(
                    _driverModels[i].Name,
                    _driverModels[i].ClassPosition,
                    _data.GetGapToPlayerMs(_driverModels[i].Idx),
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
        _driverModels = GetRelative();
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

            var aSpline =  MainWindow.IrsdkSharper.Data.GetFloat("CarIdxLapDistPct", a.Idx) *100;
            var bSpline =  MainWindow.IrsdkSharper.Data.GetFloat("CarIdxLapDistPct", b.Idx) *100;

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

        InputElement additionalDrivers = new InputElement("AdditionalRows",  _additionalDrivers.ToString());
        additionalDrivers.Margin = new Thickness(0, 0, 0, 10);
        additionalDrivers.SetValue(Grid.RowProperty, 0);

        void ParseAdditionalDriversInput(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(additionalDrivers.InputField.Text, out int maxValue))
            {
                _additionalDrivers = maxValue;
                _setIntConfig("_additionalRows", _additionalDrivers);
                _setWindowSize(336, calcHeight());
                _scaleWindow(_scale);
            }
        }

        additionalDrivers.InputField.TextChanged += ParseAdditionalDriversInput;

        Grid.SetRow(additionalDrivers, 0);
        Grid.SetColumn(additionalDrivers, 0);
        Grid.SetColumnSpan(additionalDrivers, 2);
        grid.Children.Add(additionalDrivers);


        CheckBoxElement showSessionTypeHeader = new CheckBoxElement("ShowSessionTypeHeader", _showSessionTypeHeader);
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


        CheckBoxElement showRaceDistanceHeader = new CheckBoxElement("ShowRaceDistanceHeader", _showRaceDistanceHeader);
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

        CheckBoxElement showAirTempHeader = new CheckBoxElement("ShowAirTempHeader", _showAirTempHeader);
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

        CheckBoxElement showTrackTempHeader = new CheckBoxElement("ShowTrackTempHeader", _showTrackTempHeader);
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

        CheckBoxElement showIncidentsHeader = new CheckBoxElement("ShowIncidentsHeader", _showIncidentsHeader);
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

        CheckBoxElement showSOFHeader = new CheckBoxElement("ShowSOFHeader", _showSOFHeader);
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

        CheckBoxElement showFuelHeader = new CheckBoxElement("ShowFuelHeader", _showFuelHeader);
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

        CheckBoxElement showIsWetHeader = new CheckBoxElement("ShowIsWetHeader", _showIsWetHeader);
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

        CheckBoxElement showSimTimeHeader = new CheckBoxElement("ShowSimTimeHeader", _showSimTimeHeader);
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


}