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
    private TimeSpan _lastTimeEnteredTrack;
    private bool _isOnTrack = false;
    
    // Calculated values
    private float _fuelPerLap;
    private float _fuelToFinish;
    private float _lapsToFinish;
    private float _avgLapTime;
    
    // Configs
    private float _marginLaps;
    
    
    public FuelCalculator() : base("Fuel Calculator","This Overlay calculates the fuel needed to finish")
    {
        InitializeComponent();
        _setWindowSize(170, 100);
        
        _getConfig();
        
        _lastTimeEnteredTrack = TimeSpan.Zero;
        _lastLapFuel = new List<float>();
        _lastLapTimes = new List<float>();
        
        Thread updateThread = new Thread(UpdateThreadMethod);
    
        updateThread.IsBackground = true;
        updateThread.Start();
    }
    public FuelCalculator(bool isTest) : base("","", isTest)
    {
        InitializeComponent();
        _setWindowSize(300, 30);
        
        _getConfig();
        if (!_isTest)
        {
            Thread updateThread = new Thread(UpdateThreadMethod);

            updateThread.IsBackground = true;
            updateThread.Start();
        }
        _lastLapFuel = new List<float>();
        _lastLapTimes = new List<float>();
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
        var fuelLaps = CalcFuelLaps();
        TimeInStintText.Text = (new TimeSpan(DateTime.Now.Ticks) - _lastTimeEnteredTrack).ToString(@"hh\:mm\:ss");
        FuelInTank.Text = _currentFuel.ToString("F2");
        LapsText.Text = fuelLaps.ToString("F1");
        TimeLeftText.Text = "~" + TimeSpan.FromMilliseconds(fuelLaps * _avgLapTime).ToString(@"hh\:mm\:ss");
        
        
    }

    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        
        _currentFuel = _data.LocalCarTelemetry.FuelLevel;
        Lap = _data.LocalCarTelemetry.Lap;
        OnTrack = !_data.Pitstop.InPit;

    }

    public override void UpdateThreadMethod()
    {
        try
        {
            while (true)
            {
                _getData();
                if (IsVisible)
                {

                    // Use Dispatcher to update UI from background thread
                    Dispatcher.Invoke(() => { _updateWindow(); });
                }

                // Add a small delay to prevent high CPU usage
                Thread.Sleep(64); // ~15 updates per second
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
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
                Console.WriteLine("Lap");
                _lap = value;
                OnPropertyChanged();
                OnLapChanged();
            }
        }
    }
    
    public bool OnTrack
    {
        get => _isOnTrack;
        set
        {
            if (_isOnTrack != value)
            {
                _isOnTrack = value;
                OnPropertyChanged();
                if (_isOnTrack)
                {
                    _lastTimeEnteredTrack = new TimeSpan(DateTime.Now.Ticks);
                }
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
        
        try
        {
            if (_fuelOnLastLap - _currentFuel > 0.1)
            {
                _lastLapTimes.Add(MainWindow.IrsdkSharper.Data.GetFloat("CarIdxLastLapTime", _data.PlayerIdx));
                _lastLapFuel.Add(_fuelOnLastLap - _currentFuel);
            }

            _fuelOnLastLap = _currentFuel;
            _avgLapTime = _avgLaptimeCalc();
            _fuelToFinish = CalculateFuelToEnd() - _currentFuel;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
        
    }

    private float CalculateFuelToEnd()
    {
        _fuelPerLap = _avgFuelCalc();
        if (_avgLapTime == 0)
        {
            _avgLapTime = _avgLaptimeCalc();
        }
        if (_data.SessionData.LapsTotal != 32767)
        {
            _lapsToFinish = _data.SessionData.LapsTotal - _data.LocalCarTelemetry.Lap;
        }
        else
        {
            _lapsToFinish = (float)(_data.SessionData.TimeLeft / _avgLapTime);
        }

        var neededFuel = _lapsToFinish * _fuelPerLap + (_fuelPerLap * _marginLaps);
        
        // The value is multiplied by 1.5 so that the driver can look if its possible to save fuel and 
        // stop one time less in endurance races or similar
        var maxFuel = _data.LocalCarTelemetry.FuelCapacity * 1.5f; 
        
        if (neededFuel > maxFuel)
        {
            return _data.LocalCarTelemetry.FuelCapacity * 1.5f;
        }
        return neededFuel;
    }

    private float CalcFuelLaps()
    {
        return _currentFuel / _fuelPerLap;
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

    private float _avgFuelCalc()
    {
        float fuelCalcValue = 0;
        for (int i = 0; i < _lastLapFuel.Count; i++)
        {
            fuelCalcValue += _lastLapFuel[i];
        }
        return fuelCalcValue / _lastLapFuel.Count;
    }

    private float _avgLaptimeCalc()
    {
        float lapCalcValue = 0;
        for (int i = 0; i < _lastLapTimes.Count; i++)
        {
            lapCalcValue += _lastLapTimes[i];
        }
        return lapCalcValue / _lastLapTimes.Count;
    }
}