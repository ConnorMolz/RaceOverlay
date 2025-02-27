using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using HerboldRacing;
using RaceOverlay.Data;
using RaceOverlay.Data.Models;
using RaceOverlay.Internals;
using Inputs = RaceOverlay.Overlays.Inputs.Inputs;

namespace RaceOverlay;

/// <summary>
/// Interaction Printing for MainWindow.xaml
/// </summary>

#pragma warning disable CA2211 // Non-constant fields should not be visible
public partial class MainWindow : Window
{
    // iRacingData Getter
    public static IRSDKSharper IrsdkSharper = null!;
    public static iRacingData IRacingData = new ();
    
    // Threads
    private static Thread dataThread = null!;

    // Overlays
    private Inputs _inputs = null!;
    
    public MainWindow()
    {
        InitializeComponent();
        _initIRacingData();
        _initOverlays();
        
        dataThread = new Thread(() =>
        {
            while (true)
            {
                IRacingData = Mapper.MapData(IrsdkSharper);
            }
        });
        
        dataThread.IsBackground = true;
        
    }

    private void _initOverlays()
    {
        _inputs = new Inputs();
    }

    private void _initIRacingData()
    {
        Debug.Print( "Initializing iRacing data..." );
        // create an instance of IRSDKSharper
        IrsdkSharper = new IRSDKSharper();

        // hook up our event handlers
        IrsdkSharper.OnException += OnException;
        IrsdkSharper.OnConnected += OnConnected;
        IrsdkSharper.OnDisconnected += OnDisconnected;
        IrsdkSharper.OnSessionInfo += OnSessionInfo;
        IrsdkSharper.OnTelemetryData += OnTelemetryData;
        IrsdkSharper.OnStopped += OnStopped;

        // this means fire the OnTelemetryData event every 10 data frames (6 times a second)
        IrsdkSharper.UpdateInterval = 10; 

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
        dataThread.Start();
    }

    private static void OnStopped()
    {
        dataThread.Interrupt();
        Debug.Print( "OnStopped() fired!" );
    }
    
    private void Toggle_Inputs(object sender, RoutedEventArgs e)
    {
        if (_inputs.IsVisible)
        {
            _inputs.Hide();
        }
        else
        {
            _inputs.Show();
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        Application.Current.Shutdown();
    }

    private void OverlaySelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Overlay selectedItem = ItemsListView.SelectedItem as Overlay;
            
        if (selectedItem != null)
        {
            // Get Data from Overlay
            
        }
    }
}