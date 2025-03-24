using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;
using RaceOverlay.Internals.Configs;

namespace RaceOverlay.Overlays.FuelCalculator;

public partial class FuelCalculator : Overlay
{

    // Values which gets changed on lap change
    private float _fuelOnLastLap;
    private List<float> _lastLapTimes;
    private List<float> _lastLapFuel;
    
    // Values which gets pulled by update thread
    private float _currentFuel;
    private iRacingData _data;
    private int _lap;
    
    // Calculated values
    private float _fuelPerLap;
    private float _fuelToFinish;
    private float _lapsToFinish;
    
    // Configs
    private float _marginLaps;
    
    
    public FuelCalculator() : base("Fuel Calculator","This Overlay calculates the fuel needed to finish")
    {
        InitializeComponent();
        _setWindowSize(300, 30);
        
        _getConfig();
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }
    public FuelCalculator(bool isTest) : base("","", isTest)
    {
        InitializeComponent();
        _setWindowSize(300, 30);
        
        _getConfig();
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }

    public override void _updateWindow()
    {
        if (_fuelToFinish < 0)
        {
            FuelNeededText.Background = Brushes.Green;
            FuelNeededText.Text = (_fuelToFinish * -1).ToString("F2");
        }
        else
        {
            FuelNeededText.Background = Brushes.Red;
            FuelNeededText.Text = _fuelToFinish.ToString("F2");
        }
    }

    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        
        _currentFuel = _data.LocalCarTelemetry.FuelLevel;
        _lap = _data.LocalCarTelemetry.Lap;
        
    }

    public override void UpdateThreadMethod()
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
            Thread.Sleep(64); // ~15 updates per second
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
    
    public int Lap
    {
        get => _lap;
        set
        {
            if (_lap != value)
            {
                _lap = value;
                OnPropertyChanged();
                OnLapChanged();
            }
        }
    }

    private void OnLapChanged()
    {
        if (_lastLapTimes == null)
        {
            _lastLapTimes = new List<float>();
        }
        
        if (_lastLapFuel == null)
        {
            _lastLapFuel = new List<float>();
        }
        
        if (_lastLapTimes.Count >= 10)
        {
            var newList = new List<float>();
            for (var i = 1; i < _lastLapTimes.Count; ++i)
            {
                newList.Add(_lastLapTimes[i]);
            }
            _lastLapTimes = newList;
        }

        if (_lastLapFuel.Count >= 10)
        {
            var newList = new List<float>();
            for (var i = 1; i < _lastLapFuel.Count; ++i)
            {
                newList.Add(_lastLapFuel[i]);
            }
            _lastLapFuel = newList;
        }
        
        _lastLapTimes.Add(_data.GetDriverByIdx(_data.PlayerIdx).LastLap);
        _lastLapFuel.Add(_fuelOnLastLap - _currentFuel);
        _fuelOnLastLap = _currentFuel;
        _fuelToFinish = CalculateFuelToEnd() - _currentFuel;
    }

    private float CalculateFuelToEnd()
    {
        _fuelPerLap = _lastLapFuel.Average();
        if (_data.SessionData.LapsTotal != 32767)
        {
            _lapsToFinish = _data.SessionData.LapsTotal - _data.LocalCarTelemetry.Lap;
        }
        else
        {
            var avgLapTime = _lastLapTimes.Average();
            _lapsToFinish = (float)(_data.SessionData.TimeLeft / avgLapTime);
        }

        var neededFuel = _lapsToFinish * _fuelPerLap + (_fuelPerLap * _marginLaps);
        if (neededFuel > _data.LocalCarTelemetry.FuelCapacity)
        {
            return _data.LocalCarTelemetry.FuelCapacity;
        }
        return neededFuel;
    }

    protected override void _getConfig()
    {
        if (!_isTest)
        {
            _marginLaps = _getFloatConfig("_marginLaps");
        }
    }

    public override Grid GetConfigs()
    {
        Grid grid = new Grid();
        InputElement marginLapsInputElement = new InputElement("Margin Laps: ", _marginLaps.ToString("F1"));
        
        Grid.SetRow(marginLapsInputElement, 1);
        grid.Children.Add(marginLapsInputElement);
        
        void ParseMarignInput(object sender, TextChangedEventArgs e)
        {
            if (float.TryParse(marginLapsInputElement.InputField.Text, out float marginLaps))
            {
                _marginLaps = marginLaps;
                _setFloatConfig("_marginLaps", _marginLaps);
            }
        }

        marginLapsInputElement.InputField.TextChanged += ParseMarignInput;
        return grid;
    }
}