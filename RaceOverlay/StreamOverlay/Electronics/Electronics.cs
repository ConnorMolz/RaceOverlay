using System.Windows.Controls;
using RaceOverlay.Internals.Configs;

namespace RaceOverlay.StreamOverlay.Electronics;

public class Electronics: Internals.StreamOverlay
{
    public static bool ShowABS;
    public static bool ShowTC1;
    public static bool ShowTC2;
    public static bool ShowBrakeBias;
    public static bool ShowARBFront;
    public static bool ShowARBRear;

    public Electronics() : base("Electronics",
        "An Overlay for displaying the in car adjustments of ABS, TC1, TC2, Brake Bias(BB) and Anit Roll Bars (ARB) Front and Rear.",
        "http://localhost:5480/overlay/electronics")
    {
        _getConfig();
    }

    protected override void _getConfig()
    {
        base._getConfig();
        ShowABS = _getBoolConfig("show_abs");
        ShowTC1 = _getBoolConfig("show_tc1");
        ShowTC2 = _getBoolConfig("show_tc2");
        ShowBrakeBias = _getBoolConfig("show_brake_bias");
        ShowARBFront = _getBoolConfig("show_arb_front");
        ShowARBRear = _getBoolConfig("show_arb_rear");
    }

    public override Grid GetConfig()
    {
        Grid grid = new Grid();

grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        

        CheckBoxElement absElement = new CheckBoxElement("Show ABS: ", ShowABS);
        
        absElement.CheckBox.Checked += (sender, args) =>
        {
            ShowABS = true;
            _setBoolConfig("show_abs", ShowABS);
            
        };
        absElement.CheckBox.Unchecked += (sender, args) =>
        {
            ShowABS = false;
            _setBoolConfig("show_abs", ShowABS);
            
        };
        Grid.SetRow(absElement, 1);
        Grid.SetColumn(absElement, 0);
        grid.Children.Add(absElement);
        
        CheckBoxElement tc1Element = new CheckBoxElement("Show TC1: ", ShowTC1);
        
        tc1Element.CheckBox.Checked += (sender, args) =>
        {
            ShowTC1 = true;
            _setBoolConfig("show_tc1", ShowTC1);
            
        };
        tc1Element.CheckBox.Unchecked += (sender, args) =>
        {
            ShowTC1 = false;
            _setBoolConfig("show_tc1", ShowTC1);
            
        };
        Grid.SetRow(tc1Element, 0);
        Grid.SetColumn(tc1Element, 0);
        grid.Children.Add(tc1Element);
        
        CheckBoxElement tc2Element = new CheckBoxElement("Show TC2: ", ShowTC2);
        
        tc2Element.CheckBox.Checked += (sender, args) =>
        {
            ShowTC2 = true;
            _setBoolConfig("show_tc2", ShowTC2);
            
        };
        tc2Element.CheckBox.Unchecked += (sender, args) =>
        {
            ShowTC2 = false;
            _setBoolConfig("show_tc2", ShowTC2);
            
        };
        Grid.SetRow(tc2Element, 0);
        Grid.SetColumn(tc2Element, 1);
        grid.Children.Add(tc2Element);
        
        CheckBoxElement bbElement = new CheckBoxElement("Show Brake Bias: ", ShowBrakeBias);
        bbElement.CheckBox.Checked += (sender, args) =>
        {
            ShowBrakeBias = true;
            _setBoolConfig("show_brake_bias", ShowBrakeBias);
            
        };
        bbElement.CheckBox.Unchecked += (sender, args) =>
        {
            ShowBrakeBias = false;
            _setBoolConfig("show_brake_bias", ShowBrakeBias);
            
        };
        Grid.SetRow(bbElement, 1);
        Grid.SetColumn(bbElement, 1);
        grid.Children.Add(bbElement);
        
        CheckBoxElement ARBFrontElement = new CheckBoxElement("Show ARB Front: ", ShowARBFront);
        ARBFrontElement.CheckBox.Checked += (sender, args) =>
        {
            ShowARBFront = true;
            _setBoolConfig("show_arb_front", ShowARBFront);
            
        };
        
        ARBFrontElement.CheckBox.Unchecked += (sender, args) =>
        {
            ShowARBFront = false;
            _setBoolConfig("show_arb_front", ShowARBFront);
            
        };
        Grid.SetRow(ARBFrontElement, 2);
        Grid.SetColumn(ARBFrontElement, 0);
        grid.Children.Add(ARBFrontElement);
        
        CheckBoxElement ARBRearElement = new CheckBoxElement("Show ARB Rear: ", ShowARBRear);
        ARBRearElement.CheckBox.Checked += (sender, args) =>
        {
            ShowARBRear = true;
            _setBoolConfig("show_arb_rear", ShowARBRear);
            
        };
        
        ARBRearElement.CheckBox.Unchecked += (sender, args) =>
        {
            ShowARBRear = false;
            _setBoolConfig("show_arb_rear", ShowARBRear);
            
        };
        
        Grid.SetRow(ARBRearElement, 2);
        Grid.SetColumn(ARBRearElement, 1);
        grid.Children.Add(ARBRearElement);

        return grid;
    }
}