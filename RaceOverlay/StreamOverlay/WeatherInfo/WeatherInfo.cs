namespace RaceOverlay.StreamOverlay.WeatherInfo;

public class WeatherInfo: Internals.StreamOverlay
{
    public WeatherInfo(): base(
        "Weather Info",
        "Displays the current temperature, precipitation and if the track declared to be wet from race control.",
        "http://localhost:5480/overlay/weather-info"
        ){}
}