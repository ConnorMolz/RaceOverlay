using System.Windows.Controls;
using RaceOverlay.Internals.Configs.Components;

namespace RaceOverlay.Internals.Configs;

public class CheckBoxElement: Grid
{
    public CustomLabel Label { get; set; }
    public CustomCheckBox CheckBox { get; set; }
    
    public CheckBoxElement(String text, bool isChecked)
    {
        
        RowDefinitions.Add(new RowDefinition());
        
        ColumnDefinitions.Add(new ColumnDefinition());
        ColumnDefinitions.Add(new ColumnDefinition());
        
        Label = new CustomLabel(text);
        SetColumn(Label, 0);
        SetRow(Label, 0);
        Children.Add(Label);
        
        
        CheckBox = new CustomCheckBox(isChecked);
        SetColumn(CheckBox, 1);
        SetRow(CheckBox, 0);
        Children.Add(CheckBox);
        
        
    }
    
    
}