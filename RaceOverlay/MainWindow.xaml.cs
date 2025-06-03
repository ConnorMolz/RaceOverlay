using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IRSDKSharper;
using RaceOverlay.Data;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;
using RaceOverlay.Overlays.EnergyInfo;
using RaceOverlay.Overlays.Electronics;
using RaceOverlay.Overlays.FlagPanel;
using RaceOverlay.Overlays.FuelCalculator;
using RaceOverlay.Overlays.LaptimeDelta;
using RaceOverlay.Overlays.Leaderboard;
using RaceOverlay.Overlays.PitstopInfo;
using RaceOverlay.Overlays.Relative;
using RaceOverlay.Overlays.WeatherInfo;
using RaceOverlay.Overlays.SessionInfo;
using RaceOverlay.StreamOverlay.LaptimeDelta;
using RaceOverlay.StreamOverlay.SetupHider;
using Inputs = RaceOverlay.Overlays.Inputs.Inputs;

namespace RaceOverlay;

/// <summary>
/// Main Window for the RaceOverlay application.
/// This window initializes the iRacing SDK, sets up overlays, and handles user interactions.
/// </summary>

#pragma warning disable CA2211 // Non-constant fields should not be visible
public partial class MainWindow : Window
{
    // iRacingData Getter
    public static IRacingSdk IrsdkSharper = null!;
    public static iRacingData IRacingData = new ();
    public static List<Overlay> Overlays;
    public static List<Internals.StreamOverlay> StreamOverlays;
    public static bool ShutdownIsTriggerd = false;
    
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        _initIRacingData();
        _initOverlays();
        _loadLicenseAndQuickGuide();
        App.StartApiService();
        
    }

    /// <summary>
    /// Add all overlays to the OverlayList and StreamOverlays to the StreamOverlayList.
    /// </summary>
    private void _initOverlays()
    {
        // Overlay
        MainWindow.Overlays = new List<Overlay>();
        
        // Add here every Overlay
        Overlays.Add(new Electronics());
        Overlays.Add(new EnergyInfo());
        Overlays.Add(new FlagPanel());
        Overlays.Add(new FuelCalculator());
        Overlays.Add(new Inputs());
        Overlays.Add(new LaptimeDelta());
        Overlays.Add(new Standings());
        Overlays.Add(new PitstopInfo());
        Overlays.Add(new Relative());
        //Overlays.Add(new SessionInfo());
        Overlays.Add(new WeatherInfo());

        
        Overlays = Overlays.OrderBy(o => o.OverlayName).ToList();
        
        OverlayList.ItemsSource = MainWindow.Overlays;
        
        
        // Stream Overlay
        MainWindow.StreamOverlays = new List<Internals.StreamOverlay>();
        
        // Add here every Stream Overlay
        //StreamOverlays.Add(new Test());
        StreamOverlays.Add(new BestLaptimeDelta());
        StreamOverlays.Add(new StreamOverlay.Electronics.Electronics());
        StreamOverlays.Add(new StreamOverlay.EnergyInfo.EnergyInfo());
        StreamOverlays.Add(new StreamOverlay.FlagPanel.FlagPanel());
        StreamOverlays.Add(new StreamOverlay.Inputs.Inputs());
        StreamOverlays.Add(new LastLaptimeDelta());
        StreamOverlays.Add(new SetupHider());
        StreamOverlays.Add(new StreamOverlay.WeatherInfo.WeatherInfo());

        
        StreamOverlays = StreamOverlays.OrderBy(o => o.Title).ToList();
        StreamOverlayList.ItemsSource = MainWindow.StreamOverlays;
        
        
    }

    /// <summary>
    /// Init the iRacing SDK and set up event handlers for telemetry data and session info.
    /// </summary>
    private void _initIRacingData()
    {
        Debug.Print( "Initializing iRacing data..." );
        // create an instance of IRSDKSharper
        IrsdkSharper = new IRacingSdk();

        // hook up our event handlers
        IrsdkSharper.OnException += OnException;
        IrsdkSharper.OnConnected += OnConnected;
        IrsdkSharper.OnDisconnected += OnDisconnected;
        IrsdkSharper.OnSessionInfo += OnSessionInfo;
        IrsdkSharper.OnTelemetryData += OnTelemetryData;
        IrsdkSharper.OnStopped += OnStopped;
        
        IrsdkSharper.UpdateInterval = 1; 

        // let's go!
        IrsdkSharper.Start();
    }

    /// <summary>
    /// Throws an exception if the iRacing SDK encounters an error.
    /// </summary>
    /// <param name="exception"></param>
    private static void OnException( Exception exception )
    {
        Debug.Print( "OnException() fired!" );
    }

    /// <summary>
    /// Send Debug info when the iRacing SDK is connected.
    /// </summary>
    private static void OnConnected()
    {
        Debug.Print( "OnConnected() fired!" );
    }

    /// <summary>
    /// Sends Debug info when the iRacing SDK is disconnected.
    /// </summary>
    private static void OnDisconnected()
    {
        Debug.Print( "OnDisconnected() fired!" );
    }

    /// <summary>
    /// When the session info is received, this method is called and present Debug info about the track name.
    /// </summary>
    private static void OnSessionInfo()
    {
        var trackName = IrsdkSharper.Data.SessionInfo.WeekendInfo.TrackName;
        Debug.Print( $"OnSessionInfo fired! Track name is {trackName}." );
    }

    /// <summary>
    /// Method to map the telemetry data from the iRacing SDK to the iRacingData model.
    /// </summary>
    private static void OnTelemetryData()
    {
        IRacingData = Mapper.MapData(IrsdkSharper, IRacingData);
    }

    /// <summary>
    /// Stops data mapping and resets the iRacingData model when the iRacing SDK stops. (User closed iRacing)
    /// </summary>
    private static void OnStopped()
    {
        Debug.Print( "OnStopped() fired!" );
        IRacingData = new iRacingData();
        IRacingData.InCar = false;
        
    }
    
    
    /// <summary>
    /// Method to toggle the overlay of the selected item in the OverlayList.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Toggle_Overlay(object sender, RoutedEventArgs e)
    {
        Overlay? selectedOverlay = OverlayList.SelectedItem as Overlay;
        if (selectedOverlay == null )
        {
            return;
        }
        selectedOverlay.ToggleOverlay();
    }
    

    /// <summary>
    /// Method to close the application when the window is closed.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        ShutdownIsTriggerd = true;
        Overlays = null;
        OverlayList.ItemsSource = null;
        Application.Current.Shutdown();
    }

    /// <summary>
    /// Change the shown content to the selected overlay in the OverlayList. (Conflict with StreamOverlayList if both
    /// are selected, this remove the selection from the other list)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OverlaySelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Overlay? selectedOverlay = OverlayList.SelectedItem as Overlay;
        StreamOverlayList.UnselectAll();
            
        if (selectedOverlay != null)
        {
            // Get Data from Overlay
            Debug.WriteLine(selectedOverlay.OverlayDescription);
            OverlayNameText.Text = selectedOverlay.OverlayName;
            OverlayDescriptionText.Text = selectedOverlay.OverlayDescription;
            ToggleOverlayButton.Visibility = Visibility.Visible;
            ConfigGrid.Visibility = Visibility.Visible;
            OpacityStack.Visibility = Visibility.Visible;
            ScaleStack.Visibility = Visibility.Visible;
            ScaleInput.Text = selectedOverlay.getScale().ToString("F1");
            ScaleSlider.Value = selectedOverlay.getScale();
            
            CustomConfigContainer.Children.Clear();
            Grid overlayConfigs = selectedOverlay.GetConfigs();
            CustomConfigContainer.Children.Add( overlayConfigs );

            LinkStackPanel.Visibility = Visibility.Collapsed;
            LinkTextBox.Text = string.Empty;

        }
    }
    
    /// <summary>
    /// Custom Minimize Button to minimize the window.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void minimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }
    
    /// <summary>
    /// Custom Close button.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void closeButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// This Method open the information and License page. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void goToInfoButton_Click(object sender, RoutedEventArgs e)
    {
        MainPage.Visibility = Visibility.Hidden;

        NavButtonText.Visibility = Visibility.Hidden;
        Arrow.Visibility = Visibility.Visible;
        NavButton.Click += goToMainButton_Click;
        
        InfoPage.Visibility = Visibility.Visible;
        
        
    }
    
    /// <summary>
    /// This method is called when the user clicks the "Go to Main" button on the InfoPage.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void goToMainButton_Click(object sender, RoutedEventArgs e)
    {
        MainPage.Visibility = Visibility.Visible;
        
        NavButtonText.Visibility = Visibility.Visible;
        Arrow.Visibility = Visibility.Hidden;
        NavButton.Click += goToInfoButton_Click;
        
        InfoPage.Visibility = Visibility.Hidden;
    }

    /// <summary>
    /// This method allows the user to drag the window by clicking and holding the left mouse button anywhere on the window.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
    
    /// <summary>
    /// Loading the LICENSE and Quick Guide from the embedded resources.
    /// </summary>
    private void _loadLicenseAndQuickGuide()
    {
        try
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            
            string resourceNameLicense = "RaceOverlay.Resources.LICENSE";
                
            using (Stream stream = assembly.GetManifestResourceStream(resourceNameLicense))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        LicenseText.Text = reader.ReadToEnd();
                    }
                }
                else
                {
                    LicenseText.Text = "LICENSE resource not found in the application.";
                }
            }
            string resourceNameManual = "RaceOverlay.Resources.Manual";
                
            using (Stream stream = assembly.GetManifestResourceStream(resourceNameManual))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        QuickGuideText.Text = reader.ReadToEnd();
                    }
                }
                else
                {
                    LicenseText.Text = "LICENSE resource not found in the application.";
                }
            }
        }
        catch (Exception ex)
        {
            LicenseText.Text = $"Error loading LICENSE resource: {ex.Message}";
        }
    }
    
    /// <summary>
    /// Method to handle the value change of the ScaleSlider and scale the selected overlay accordingly.
    /// (This method is triggered by the ScaleInput_TextChanged event as well, so it will update the text box value if the slider is changed manually.)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ScaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        double scale = ScaleSlider.Value;
            
        Overlay? selectedOverlay = OverlayList.SelectedItem as Overlay;
            
        if (selectedOverlay != null)
        {
            selectedOverlay.ScaleValueChanges(scale);
            
        }
            
        // Update text box to match (without triggering its event)
        ScaleInput.TextChanged -= ScaleInput_TextChanged;
        ScaleInput.Text = scale.ToString("F1");
        ScaleInput.TextChanged += ScaleInput_TextChanged;
    }
    
    /// <summary>
    /// This method is called when the text in the ScaleInput TextBox changes and change the scale of the selected overlay accordingly.
    /// (This method is triggered by the ScaleSlider_ValueChanged event as well, so it will update the slider value if the text is changed manually.)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ScaleInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ConfigGrid.Visibility != Visibility.Visible)
        {
            return;
        }

        if (float.TryParse(ScaleInput.Text, out float scale))
        {
            // Ensure _scale is within reasonable bounds
            scale = Math.Max(0.5f, Math.Min(scale, 2.0f));
                
            Overlay? selectedOverlay = OverlayList.SelectedItem as Overlay;
            
            if (selectedOverlay != null)
            {
                selectedOverlay.ScaleValueChanges(scale);
            
            }
                
            // Update slider to match (without triggering its event)
            ScaleSlider.ValueChanged -= ScaleSlider_ValueChanged;
            ScaleSlider.Value = Math.Max(ScaleSlider.Minimum, Math.Min(scale, ScaleSlider.Maximum));
            ScaleSlider.ValueChanged += ScaleSlider_ValueChanged;
        }
    }

    /// <summary>
    /// Static getter to get iRacingSdk instance to safe resources.
    /// </summary>
    /// <returns></returns>
    public static IRacingSdk getRSDK()
    {
        return IrsdkSharper;
    }
    
    /// <summary>
    /// This method is called when the value of the OpacitySlider changes and updates the opacity of the selected overlay accordingly.
    /// (This method is triggered by the OpacityInput_TextChanged event as well, so it will update the text box value if the slider is changed manually.)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        double opacity = OpacitySlider.Value;
            
        Overlay? selectedOverlay = OverlayList.SelectedItem as Overlay;
            
        if (selectedOverlay != null)
        {
            selectedOverlay.OpacityValueChanges(opacity);
            
        }
            
        // Update text box to match (without triggering its event)
        OpacityInput.TextChanged -= OpacityInput_TextChanged;
        OpacityInput.Text = opacity.ToString("F1");
        OpacityInput.TextChanged += OpacityInput_TextChanged;
    }
    
    /// <summary>
    /// This method is called when the text in the OpacityInput TextBox changes and updates the opacity of the selected overlay accordingly.
    /// (This method is triggered by the OpacitySlider_ValueChanged event as well, so it will update the slider value if the text is changed manually.)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpacityInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ConfigGrid.Visibility != Visibility.Visible)
        {
            return;
        }

        if (float.TryParse(OpacityInput.Text, out float opacity))
        {
            // Ensure _scale is within reasonable bounds
            opacity = Math.Max(0.5f, Math.Min(opacity, 2.0f));
                
            Overlay? selectedOverlay = OverlayList.SelectedItem as Overlay;
            
            if (selectedOverlay != null)
            {
                selectedOverlay.OpacityValueChanges(opacity);
            
            }
                
            // Update slider to match (without triggering its event)
            OpacitySlider.ValueChanged -= OpacitySlider_ValueChanged;
            OpacitySlider.Value = Math.Max(ScaleSlider.Minimum, Math.Min(opacity, ScaleSlider.Maximum));
            OpacitySlider.ValueChanged += OpacitySlider_ValueChanged;
        }
    }

    /// <summary>
    /// Change the shown content to the selected stream overlay in the StreamOverlayList. (Conflict with OverlayList if both
    /// are selected, this remove the selection from the other list)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StreamOverlayList_OnSelectionChangedOverlaySelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Internals.StreamOverlay? selectedOverlay = StreamOverlayList.SelectedItem as Internals.StreamOverlay;
        OverlayList.UnselectAll();
            
        if (selectedOverlay != null)
        {
            // Get Data from Overlay
            Debug.WriteLine(selectedOverlay.Title);
            OverlayNameText.Text = selectedOverlay.Title;
            OverlayDescriptionText.Text = selectedOverlay.Description;
            ConfigGrid.Visibility = Visibility.Visible;
            LinkStackPanel.Visibility = Visibility.Visible;
            LinkTextBox.Visibility = Visibility.Visible;
            OpacityStack.Visibility = Visibility.Collapsed;
            ScaleStack.Visibility = Visibility.Collapsed;
            
            LinkTextBox.Text = selectedOverlay.Link;
            CustomConfigContainer.Children.Clear();
            CustomConfigContainer.Children.Add(selectedOverlay.GetConfig());
            
            ToggleOverlayButton.Visibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// Handle the click event of the CopyLinkButton to copy the link from the LinkTextBox to the clipboard.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyLinkButtonMethod(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Hello");
        Clipboard.SetText(LinkTextBox.Text);
    }
}