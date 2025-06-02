using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using Path = System.IO.Path;
using ShapesPath = System.Windows.Shapes.Path;

namespace RaceOverlay.StreamOverlay.SetupHider;

public class SetupHider: Internals.StreamOverlay
{
    public SetupHider() : base("Setup Hider",
        "This Overlay provides an hider for setups while car is in Garage.",
        "http://localhost:5480/overlay/setup_hider")
    {
    }

    public override Grid GetConfig()
    {
        Grid grid = new Grid();

        // Create the main button
            Button uploadButton = new Button
            {
                Width = 240,
                Height = 40,
                Background = new SolidColorBrush(Color.FromRgb(51, 51, 51)), // Dark gray
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(20)
            };

            // Create rounded corners
            uploadButton.Style = CreateButtonStyle();

            // Create the content panel
            StackPanel contentPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Create upload icon (using a simple path geometry)
            ShapesPath uploadIcon = new ShapesPath
            {
                Data = Geometry.Parse("M12 4L12 14 M8 8L12 4 16 8 M4 18L20 18"),
                Stroke = Brushes.White,
                StrokeThickness = 1.5,
                StrokeLineJoin = PenLineJoin.Round,
                Width = 16,
                Height = 16,
                Margin = new Thickness(0, 0, 8, 0),
                Stretch = Stretch.Uniform
            };

            // Create text label
            TextBlock uploadText = new TextBlock
            {
                Text = "Upload new Setup Hider Image",
                Foreground = Brushes.White,
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 13,
                FontWeight = FontWeights.Medium,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Add icon and text to content panel
            contentPanel.Children.Add(uploadIcon);
            contentPanel.Children.Add(uploadText);

            // Set button content
            uploadButton.Content = contentPanel;

            // Add click event handler
            uploadButton.Click += UploadButton_Click;
        grid.Children.Add(uploadButton);
        return grid;
    }

    private Style CreateButtonStyle()
    {
        Style buttonStyle = new Style(typeof(Button));

        // Set the template to create rounded corners and hover effects
        ControlTemplate template = new ControlTemplate(typeof(Button));

        FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
        border.Name = "border";
        border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
        border.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Button.BorderBrushProperty));
        border.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(Button.BorderThicknessProperty));
        border.SetValue(Border.CornerRadiusProperty, new CornerRadius(20));

        FrameworkElementFactory contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
        contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

        border.AppendChild(contentPresenter);
        template.VisualTree = border;

        // Add triggers for hover effect
        Trigger mouseOverTrigger = new Trigger
        {
            Property = Button.IsMouseOverProperty,
            Value = true
        };
        mouseOverTrigger.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Color.FromRgb(71, 71, 71))));

        template.Triggers.Add(mouseOverTrigger);

        buttonStyle.Setters.Add(new Setter(Button.TemplateProperty, template));

        return buttonStyle;
    }


    private void UploadButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Create file dialog for JPG files only
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Select JPG Image",
                Filter = "JPG Images (*.jpg)|*.jpg",
                FilterIndex = 1,
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedFile = dialog.FileName;

                // Verify the file is actually a JPG (double-check extension)
                if (!Path.GetExtension(selectedFile).Equals(".jpg", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Only JPG files are allowed.", "Invalid File Type",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Verwende immer den festen Namen "SetupHider.jpg"
                string destinationPath = Path.Combine(App.AppDataPath, "SetupHider.jpg");

                // Überschreibe die Datei, wenn sie existiert
                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath);
                }

                // Kopiere die Datei
                File.Copy(selectedFile, destinationPath);

                MessageBox.Show($"JPG image uploaded successfully!\nSaved to: {destinationPath}",
                    "Upload Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error uploading file: {ex.Message}", "Upload Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}