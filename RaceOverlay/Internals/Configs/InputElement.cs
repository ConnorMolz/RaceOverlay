using System.Windows.Controls;
using RaceOverlay.Internals.Configs.Components;

namespace RaceOverlay.Internals.Configs;

public class InputElement: Grid
{
    public CustomLabel Label { get; set; }
    public CustomInputField InputField { get; set; }

    public InputElement(String labelText, String inputFieldText)
    {
        ColumnDefinitions.Add(new ColumnDefinition());
        ColumnDefinitions.Add(new ColumnDefinition());
        
        Label = new CustomLabel(labelText);
        SetColumn(Label, 0);
        Children.Add(Label);
        
        InputField = new CustomInputField(inputFieldText);
        SetColumn(InputField, 1);
        Children.Add(InputField);
    }
}