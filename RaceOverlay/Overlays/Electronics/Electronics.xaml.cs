using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;

namespace RaceOverlay.Overlays.Electronics;

public partial class Electronics : Overlay
{

    private float _abs;
    private float _tc1;
    private float _tc2;
    private float _brakeBias;

    private bool _showBB;
    private bool _showTc1;
    private bool _showTc2;
    private bool _showAbs;

    private int _width = 0;
    private  readonly int _fieldBaseWidth = 40;

    private iRacingData _data;
    
    public Electronics(): base("Electronics", "An Overlay for displaying the in car adjustments of ABS, TC1, TC2 and Brake Bias(BB).")
    {
        InitializeComponent();
        _getConfig();

        _setWindowSize(calcWindowWidth(), 65);
        
        Thread updateThread = new Thread(UpdateThreadMethod);
        
        updateThread.IsBackground = true;
        updateThread.Start();
    }
    
    public override void _getData()
    {
        _data = MainWindow.IRacingData;
        _abs = _data.LocalCarTelemetry.Abs;
        _tc1 = _data.LocalCarTelemetry.Tc1;
        _tc2 = _data.LocalCarTelemetry.Tc2;
        _brakeBias = _data.LocalCarTelemetry.BrakeBias;
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
        if (_showAbs)
        {
            absValue.Text = _abs.ToString("F0");
        }

        if (_showTc1)
        {
            tc1Value.Text = _tc1.ToString("F0");
        }

        if (_showTc2)
        {
            tc2Value.Text = _tc2.ToString("F0");
        }

        if (_showBB)
        {
            bbValue.Text = _brakeBias.ToString("F2");
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
    
    public int WindowWidth
    {
        get => _width;
        set
        {
            if (_width != value)
            {
                _width = value;
                OnPropertyChanged();
                OnWidthChanged();
            }
        }
    }
    
    private void OnWidthChanged()
    {
        _setWindowSize(WindowWidth, 65);
    }

    public int calcWindowWidth()
    {
        absText.Visibility = Visibility.Collapsed;
        absValue.Visibility = Visibility.Collapsed;
        
        tc1Text.Visibility = Visibility.Collapsed;
        tc1Value.Visibility = Visibility.Collapsed;
        
        tc2Text.Visibility = Visibility.Collapsed;
        tc2Value.Visibility = Visibility.Collapsed;
        
        bbText.Visibility = Visibility.Collapsed;
        bbValue.Visibility = Visibility.Collapsed;
        
        int size = 0;
        if (_showAbs)
        {
            absText.Visibility = Visibility.Visible;
            absValue.Visibility = Visibility.Visible;
            size += _fieldBaseWidth;
        }
        if (_showTc1)
        {
            tc1Text.Visibility = Visibility.Visible;
            tc1Value.Visibility = Visibility.Visible;
            size += _fieldBaseWidth;
        }
        if (_showTc2)
        {
            tc2Text.Visibility = Visibility.Visible;
            tc2Value.Visibility = Visibility.Visible;
            size += _fieldBaseWidth;
        }
        if (_showBB)
        {
            bbText.Visibility = Visibility.Visible;
            bbValue.Visibility = Visibility.Visible;
            size += _fieldBaseWidth;
        }

        return size;
    }

    protected override void _getConfig()
    {
        _showAbs = _getBoolConfig("_showAbs");
        _showTc1 = _getBoolConfig("_showTc1");
        _showTc2 = _getBoolConfig("_showTc2");
        _showBB = _getBoolConfig("_showBB");
    }

    public override Grid GetConfigs()
    {
        Grid grid = new Grid();
        
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());

        TextBlock absLabel = new TextBlock();
        absLabel.Text = "Show ABS: ";
        Grid.SetColumn(absLabel, 0);
        Grid.SetRow(absLabel, 0);
        grid.Children.Add(absLabel);
        
        CheckBox absCheck = new CheckBox();
        absCheck.IsChecked = _showAbs;
        absCheck.Checked += (sender, args) =>
        {
            _showAbs = true;
            _setBoolConfig("_showAbs", _showAbs);
            WindowWidth = calcWindowWidth();
        };
        absCheck.Unchecked += (sender, args) =>
        {
            _showAbs = false;
            _setBoolConfig("_showAbs", _showAbs);
            WindowWidth = calcWindowWidth();
        };
        Grid.SetColumn(absCheck, 1);
        Grid.SetRow(absCheck, 0);
        grid.Children.Add(absCheck);
        
        TextBlock tc1Label = new TextBlock();
        tc1Label.Text = "Show TC1: ";
        Grid.SetColumn(tc1Label, 0);
        Grid.SetRow(tc1Label, 1);
        grid.Children.Add(tc1Label);
        
        CheckBox tc1Check = new CheckBox();
        tc1Check.IsChecked = _showTc1;
        tc1Check.Checked += (sender, args) =>
        {
            _showTc1 = true;
            _setBoolConfig("_showTc1", _showTc1);
            WindowWidth = calcWindowWidth();
        };
        tc1Check.Unchecked += (sender, args) =>
        {
            _showTc1 = false;
            _setBoolConfig("_showTc1", _showTc1);
            WindowWidth = calcWindowWidth();
        };
        Grid.SetColumn(tc1Check, 1);
        Grid.SetRow(tc1Check, 1);
        grid.Children.Add(tc1Check);
        
        TextBlock tc2Label = new TextBlock();
        tc2Label.Text = "Show TC2: ";
        Grid.SetColumn(tc2Label, 0);
        Grid.SetRow(tc2Label, 2);
        grid.Children.Add(tc2Label);
        
        CheckBox tc2Check = new CheckBox();
        tc2Check.IsChecked = _showTc2;
        tc2Check.Checked += (sender, args) =>
        {
            _showTc2 = true;
            _setBoolConfig("_showTc2", _showTc2);
            WindowWidth = calcWindowWidth();
        };
        tc2Check.Unchecked += (sender, args) =>
        {
            _showTc2 = false;
            _setBoolConfig("_showTc2", _showTc2);
            WindowWidth = calcWindowWidth();
        };
        Grid.SetColumn(tc2Check, 1);
        Grid.SetRow(tc2Check, 2);
        grid.Children.Add(tc2Check);
        
        TextBlock bbLabel = new TextBlock();
        bbLabel.Text = "Show Brake Bias: ";
        Grid.SetColumn(bbLabel, 0);
        Grid.SetRow(bbLabel, 3);
        grid.Children.Add(bbLabel);
        
        CheckBox bbCheck = new CheckBox();
        bbCheck.IsChecked = _showBB;
        bbCheck.Checked += (sender, args) =>
        {
            _showBB = true;
            _setBoolConfig("_showBB", _showBB);
            WindowWidth = calcWindowWidth();
        };
        bbCheck.Unchecked += (sender, args) =>
        {
            _showBB = false;
            _setBoolConfig("_showBB", _showBB);
            WindowWidth = calcWindowWidth();
        };
        Grid.SetColumn(bbCheck, 1);
        Grid.SetRow(bbCheck, 3);
        grid.Children.Add(bbCheck);

        return grid;
    }
}