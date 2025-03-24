using System.Windows;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.FuelCalculator;

public partial class FuelCalculator : Overlay
{

    // Values which gets changed on lap change
    private iRacingData _data;
    private int _lap;
    private float _fuelOnLastLap;
    private List<float> _lastLapTimes;
    private List<float> _lastLapFuel;
    
    // Values which gets pulled by update thread
    private float _currentFuel;
    
    // Calculated values
    private float _fuelPerLap;
    private float _fuelToFinish;
    private float _lapsToFinish;
    
    // Configs
    private float _marginLaps;
    
    
    public FuelCalculator() : base("","")
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
        //throw new NotImplementedException();
    }

    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        
        _currentFuel = _data.LocalCarTelemetry.FuelLevel;
        _lap = _data.LocalCarTelemetry.Lap;
        
        
    }

    public override void UpdateThreadMethod()
    {
        //throw new NotImplementedException();
    }

    protected override void _scaleWindow(double scale)
    {
        throw new NotImplementedException();
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
        _fuelToFinish = CalculateFuelToEnd();
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
    
}