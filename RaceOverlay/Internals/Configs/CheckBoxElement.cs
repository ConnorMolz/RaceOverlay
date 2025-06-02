using System.Windows.Controls;
using RaceOverlay.Internals.Configs.Components;

namespace RaceOverlay.Internals.Configs;

/// <summary>
/// Element which contains a Label and a CheckBox for simple Checkbox configs.
/// </summary>
/// <returns>This is an extended Grid class and can be used as Grid by default</returns>
public class CheckBoxElement: Grid
{
    public CustomLabel Label { get; set; }
    public CustomCheckBox CheckBox { get; set; }
    
    /// <summary>
    /// Constructor for CheckBoxElement.
    /// </summary>
    /// <param name="text">The text which will be shown as label for the checkbox</param>
    /// <param name="isChecked">Value which is used for the checkbox if it's checked on show</param>
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