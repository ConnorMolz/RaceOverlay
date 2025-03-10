using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IRSDKSharper;
using RaceOverlay.Data;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;
using RaceOverlay.Overlays.EnergyInfo;
using RaceOverlay.Overlays.Electronics;
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
    private List<Overlay> Overlays;
    public static bool ShutdownIsTriggerd = false;
    
    
    public MainWindow()
    {
        InitializeComponent();
        _initIRacingData();
        _initOverlays();
        
    }

    private void _initOverlays()
    {
        Overlays = new List<Overlay>();
        
        // Add here every Overlay
        Overlays.Add(new Electronics());
        Overlays.Add(new EnergyInfo());
        Overlays.Add(new Inputs());
        Overlays.Add(new SessionInfo());
        Overlays.Add(new WeatherInfo());
        
        
        OverlayList.ItemsSource = Overlays;
        
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
        
    }
    
    private void goToMainButton_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}