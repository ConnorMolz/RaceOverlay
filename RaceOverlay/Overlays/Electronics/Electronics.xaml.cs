using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;
using RaceOverlay.Internals.Configs;

namespace RaceOverlay.Overlays.Electronics;

public partial class Electronics : Overlay
{

    private float _abs;
    private float _tc1;
    private float _tc2;
    private float _brakeBias;
    private float _ARBFront;
    private float _ARBRear;

    private bool _showBB;
    private bool _showTc1;
    private bool _showTc2;
    private bool _showAbs;
    private bool _showARBFront;
    private bool _showARBRear;

    private int _width = 0;
    private readonly int _smallFieldBaseWidth = 40;
    private readonly int _largeFieldBaseWidth = 52;

    private iRacingData _data;
    
    public Electronics(): base("Electronics", "An Overlay for displaying the in car adjustments of ABS, TC1, TC2, Brake Bias(BB) and Anit Roll Bars (ARB) Front and Rear.")
    {
        InitializeComponent();
        _getConfig();

        _setWindowSize(calcWindowWidth(), 68);
        
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
        _ARBFront = _data.LocalCarTelemetry.ARBFront;
        _ARBRear = _data.LocalCarTelemetry.ARBRear;
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
        
        if (_showARBFront)
        {
            ARBFValue.Text = _ARBFront.ToString("F0");
        }
        
        if (_showARBRear)
        {
            ARBRValue.Text = _ARBRear.ToString("F0");
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
        _setWindowSize(WindowWidth, 68);
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

        ARBFText.Visibility = Visibility.Collapsed;
        ARBFValue.Visibility = Visibility.Collapsed;
        
        ARBRText.Visibility = Visibility.Collapsed;
        ARBRValue.Visibility = Visibility.Collapsed;
        
        int size = 0;
        if (_showAbs)
        {
            absText.Visibility = Visibility.Visible;
            absValue.Visibility = Visibility.Visible;
            size += _smallFieldBaseWidth;
        }
        if (_showTc1)
        {
            tc1Text.Visibility = Visibility.Visible;
            tc1Value.Visibility = Visibility.Visible;
            size += _smallFieldBaseWidth;
        }
        if (_showTc2)
        {
            tc2Text.Visibility = Visibility.Visible;
            tc2Value.Visibility = Visibility.Visible;
            size += _smallFieldBaseWidth;
        }
        if (_showBB)
        {
            bbText.Visibility = Visibility.Visible;
            bbValue.Visibility = Visibility.Visible;
            size += _smallFieldBaseWidth;
        }

        if (_showARBFront)
        {
            ARBFText.Visibility = Visibility.Visible;
            ARBFValue.Visibility = Visibility.Visible;
            size += _largeFieldBaseWidth;
        }
        
        if (_showARBRear)
        {
            ARBRText.Visibility = Visibility.Visible;
            ARBRValue.Visibility = Visibility.Visible;
            size += _largeFieldBaseWidth;
        }

        if (size == 0)
        {
            Hide();
        }

        if (size != 0 && _windowIsActive && _inCar)
        {
            Show();
        }
        
        return size;
    }

    protected override void _getConfig()
    {
        _showAbs = _getBoolConfig("_showAbs");
        _showTc1 = _getBoolConfig("_showTc1");
        _showTc2 = _getBoolConfig("_showTc2");
        _showBB = _getBoolConfig("_showBB");
        _showARBFront = _getBoolConfig("_showARBFront");
        _showARBRear = _getBoolConfig("_showARBRear");
    }

    public override Grid GetConfigs()
    {
        Grid grid = new Grid();
        
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        

        CheckBoxElement absElement = new CheckBoxElement("Show ABS: ", _showAbs);
        
        absElement.CheckBox.Checked += (sender, args) =>
        {
            _showAbs = true;
            _setBoolConfig("_showAbs", _showAbs);
            WindowWidth = calcWindowWidth();
        };
        absElement.CheckBox.Unchecked += (sender, args) =>
        {
            _showAbs = false;
            _setBoolConfig("_showAbs", _showAbs);
            WindowWidth = calcWindowWidth();
        };
        Grid.SetRow(absElement, 1);
        Grid.SetColumn(absElement, 0);
        grid.Children.Add(absElement);
        
        CheckBoxElement tc1Element = new CheckBoxElement("Show TC1: ", _showTc1);
        
        tc1Element.CheckBox.Checked += (sender, args) =>
        {
            _showTc1 = true;
            _setBoolConfig("_showTc1", _showTc1);
            WindowWidth = calcWindowWidth();
        };
        tc1Element.CheckBox.Unchecked += (sender, args) =>
        {
            _showTc1 = false;
            _setBoolConfig("_showTc1", _showTc1);
            WindowWidth = calcWindowWidth();
        };
        Grid.SetRow(tc1Element, 0);
        Grid.SetColumn(tc1Element, 0);
        grid.Children.Add(tc1Element);
        
        CheckBoxElement tc2Element = new CheckBoxElement("Show TC2: ", _showTc2);
        
        tc2Element.CheckBox.Checked += (sender, args) =>
        {
            _showTc2 = true;
            _setBoolConfig("_showTc2", _showTc2);
            WindowWidth = calcWindowWidth();
        };
        tc2Element.CheckBox.Unchecked += (sender, args) =>
        {
            _showTc2 = false;
            _setBoolConfig("_showTc2", _showTc2);
            WindowWidth = calcWindowWidth();
        };
        Grid.SetRow(tc2Element, 0);
        Grid.SetColumn(tc2Element, 1);
        grid.Children.Add(tc2Element);
        
        CheckBoxElement bbElement = new CheckBoxElement("Show Brake Bias: ", _showBB);
        bbElement.CheckBox.Checked += (sender, args) =>
        {
            _showBB = true;
            _setBoolConfig("_showBB", _showBB);
            WindowWidth = calcWindowWidth();
        };
        bbElement.CheckBox.Unchecked += (sender, args) =>
        {
            _showBB = false;
            _setBoolConfig("_showBB", _showBB);
            WindowWidth = calcWindowWidth();
        };
        Grid.SetRow(bbElement, 1);
        Grid.SetColumn(bbElement, 1);
        grid.Children.Add(bbElement);
        
        CheckBoxElement ARBFrontElement = new CheckBoxElement("Show ARB Front: ", _showARBFront);
        ARBFrontElement.CheckBox.Checked += (sender, args) =>
        {
            _showARBFront = true;
            _setBoolConfig("_showARBFront", _showARBFront);
            WindowWidth = calcWindowWidth();
        };
        
        ARBFrontElement.CheckBox.Unchecked += (sender, args) =>
        {
            _showARBFront = false;
            _setBoolConfig("_showARBFront", _showARBFront);
            WindowWidth = calcWindowWidth();
        };
        Grid.SetRow(ARBFrontElement, 2);
        Grid.SetColumn(ARBFrontElement, 0);
        grid.Children.Add(ARBFrontElement);
        
        CheckBoxElement ARBRearElement = new CheckBoxElement("Show ARB Rear: ", _showARBRear);
        ARBRearElement.CheckBox.Checked += (sender, args) =>
        {
            _showARBRear = true;
            _setBoolConfig("_showARBRear", _showARBRear);
            WindowWidth = calcWindowWidth();
        };
        
        ARBRearElement.CheckBox.Unchecked += (sender, args) =>
        {
            _showARBRear = false;
            _setBoolConfig("_showARBRear", _showARBRear);
            WindowWidth = calcWindowWidth();
        };
        
        Grid.SetRow(ARBRearElement, 2);
        Grid.SetColumn(ARBRearElement, 1);
        grid.Children.Add(ARBRearElement);

        return grid;
    }
}