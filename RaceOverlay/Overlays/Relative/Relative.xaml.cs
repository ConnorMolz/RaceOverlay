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
    
    
    //TODO: Add Description
    public Relative() : base("Relative", "")
    {
        InitializeComponent();
    }

    public override void _updateWindow()
    {
        for (int i = 0; i < _driverModels.Count; i++)
        {
            Body.Children.Add(new RelativeRow(
                _driverModels[i].Name,
                _driverModels[i].Position,
                Math.Abs(_data.GetGapToPlayerMs(_driverModels[i].Idx)),
                _driverModels[i].CarNumber,
                _driverModels[i].ClassColorCode
            ));
        }
    }

    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        if (_data == null)
            return;
        _driverModels = GetRelative();
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
            }
        }
        
        additionalDrivers.InputField.TextChanged += ParseAdditionalDriversInput;
        
        grid.Children.Add(additionalDrivers);
        
        return grid;
    }
}