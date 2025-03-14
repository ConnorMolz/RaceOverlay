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
using RaceOverlay.Overlays.LaptimeDelta;
using RaceOverlay.Overlays.WeatherInfo;
using RaceOverlay.Overlays.SessionInfo;
using Inputs = RaceOverlay.Overlays.Inputs.Inputs;

namespace RaceOverlay;

/// <summary>
/// Interaction Printing for MainWindow.xaml
/// </summary>

#pragma warning disable CA2211 // Non-constant fields should not be visible
public partial class MainWindow : Window
{
    // iRacingData Getter
    private static IRacingSdk IrsdkSharper = null!;
    public static iRacingData IRacingData = new ();
    public static List<Overlay> Overlays;
    public static bool ShutdownIsTriggerd = false;
    
    
    public MainWindow()
    {
        InitializeComponent();
        _initIRacingData();
        _initOverlays();
        _loadLicenseAndQuickGuide();
        
    }

    private void _initOverlays()
    {
        MainWindow.Overlays = new List<Overlay>();
        
        // Add here every Overlay
        MainWindow.Overlays.Add(new Electronics());
        MainWindow.Overlays.Add(new EnergyInfo());
        MainWindow.Overlays.Add(new Inputs());
        MainWindow.Overlays.Add(new LaptimeDelta());
        MainWindow.Overlays.Add(new SessionInfo());
        MainWindow.Overlays.Add(new WeatherInfo());
        
        
        OverlayList.ItemsSource = MainWindow.Overlays;
        
    }

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
        
        IrsdkSharper.UpdateInterval = 0; 

        // let's go!
        IrsdkSharper.Start();
    }
    
    private void Window_Closing( object sender, CancelEventArgs e )
    {
        IrsdkSharper.Stop();
    }

    private static void OnException( Exception exception )
    {
        Debug.Print( "OnException() fired!" );
    }

    private static void OnConnected()
    {
        Debug.Print( "OnConnected() fired!" );
    }

    private static void OnDisconnected()
    {
        Debug.Print( "OnDisconnected() fired!" );
        
        
    }

    private static void OnSessionInfo()
    {
        var trackName = IrsdkSharper.Data.SessionInfo.WeekendInfo.TrackName;
        Debug.Print( $"OnSessionInfo fired! Track name is {trackName}." );
    }

    private static void OnTelemetryData()
    {
        IRacingData = Mapper.MapData(IrsdkSharper);
    }

    private static void OnStopped()
    {
        Debug.Print( "OnStopped() fired!" );
        IRacingData.InCar = false;
    }
    
    
    
    private void Toggle_Overlay(object sender, RoutedEventArgs e)
    {
        Overlay? selectedOverlay = OverlayList.SelectedItem as Overlay;
        if (selectedOverlay == null )
        {
            return;
        }
        selectedOverlay.ToggleOverlay();
    }
    

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        ShutdownIsTriggerd = true;
        Overlays = null;
        OverlayList.ItemsSource = null;
        Application.Current.Shutdown();
    }

    private void OverlaySelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Overlay? selectedOverlay = OverlayList.SelectedItem as Overlay;
            
        if (selectedOverlay != null)
        {
            // Get Data from Overlay
            Debug.WriteLine(selectedOverlay.OverlayDescription);
            OverlayNameText.Text = selectedOverlay.OverlayName;
            OverlayDescriptionText.Text = selectedOverlay.OverlayDescription;
            ToggleOverlayButton.Visibility = Visibility.Visible;
            ConfigGrid.Visibility = Visibility.Visible;
            ScaleInput.Text = selectedOverlay.getScale().ToString("F1");
            ScaleSlider.Value = selectedOverlay.getScale();
            
            CustomConfigContainer.Children.Clear();
            Grid overlayConfigs = selectedOverlay.GetConfigs();
            CustomConfigContainer.Children.Add( overlayConfigs );

        }
    }
    
    private void minimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }
    
    private void closeButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void goToInfoButton_Click(object sender, RoutedEventArgs e)
    {
        MainPage.Visibility = Visibility.Hidden;

        NavButtonText.Visibility = Visibility.Hidden;
        Arrow.Visibility = Visibility.Visible;
        NavButton.Click += goToMainButton_Click;
        
        InfoPage.Visibility = Visibility.Visible;
        
        
    }
    
    private void goToMainButton_Click(object sender, RoutedEventArgs e)
    {
        MainPage.Visibility = Visibility.Visible;
        
        NavButtonText.Visibility = Visibility.Visible;
        Arrow.Visibility = Visibility.Hidden;
        NavButton.Click += goToInfoButton_Click;
        
        InfoPage.Visibility = Visibility.Hidden;
    }

    private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
    
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
    
}