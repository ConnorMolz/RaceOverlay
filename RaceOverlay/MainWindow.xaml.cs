using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using HerboldRacing;
using RaceOverlay.Overlays.Inputs;

namespace RaceOverlay;

/// <summary>
/// Interaction Printic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public static IRSDKSharper IrsdkSharper = null!;

    private Inputs _inputs;
    public MainWindow()
    {
        InitializeComponent();
        _initIRacingData();
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

        // this means fire the OnTelemetryData event every 30 data frames (2 times a second)
        IrsdkSharper.UpdateInterval = 30; 

        // lets go!
        IrsdkSharper.Start();
    }
    
    private void Window_Closing( object sender, CancelEventArgs e )
    {
        IrsdkSharper.Stop();
    }

    private void OnException( Exception exception )
    {
        Debug.Print( "OnException() fired!" );
    }

    private void OnConnected()
    {
        Debug.Print( "OnConnected() fired!" );
    }

    private void OnDisconnected()
    {
        Debug.Print( "OnDisconnected() fired!" );
    }

    private void OnSessionInfo()
    {
        var trackName = IrsdkSharper.Data.SessionInfo.WeekendInfo.TrackName;

        Debug.Print( $"OnSessionInfo fired! Track name is {trackName}." );
    }

    private void OnTelemetryData()
    {
        var lapDistPct = IrsdkSharper.Data.GetFloat( "CarIdxLapDistPct", 5 );

        Debug.Print( $"OnTelemetryData fired! Lap dist pct for the 6th car in the array is {lapDistPct}." );
    }

    private static void OnStopped()
    {
        Debug.Print( "OnStopped() fired!" );
    }
    
    private void Open_Inputs(object sender, RoutedEventArgs e)
    {
        _inputs.Show();
    }
}