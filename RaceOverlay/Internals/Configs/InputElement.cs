using System.Windows.Controls;
using RaceOverlay.Internals.Configs.Components;

namespace RaceOverlay.Internals.Configs;

/// <summary>
/// Element which contains a Label and a Text field for text configs. <br/>
/// To use it you need to add some eventhandlers on the InputField to get the text changes and validate those. <br/>
/// </summary>
/// <returns>This is an extended Grid class and can be used as Grid by default</returns>
public class InputElement: Grid
{
    public CustomLabel Label { get; set; }
    public CustomInputField InputField { get; set; }

    /// <summary>
    /// Constructor for InputElement.
    /// </summary>
    /// <param name="labelText">Label Text which is shown in front of the TextInput.</param>
    /// <param name="inputFieldText">Text which is set at the Field at create time.</param>
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