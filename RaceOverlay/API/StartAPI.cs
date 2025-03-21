using System.Diagnostics;
using System.IO;
using System.Windows;
using API_test.Overlays.Inputs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RaceOverlay.API;

public class StartAPI
{
    public static IHost StartApiServer()
    {
        Debug.WriteLine("API IS STARTING...");
        var assembly = typeof(StartAPI).Assembly;
        foreach (var name in assembly.GetManifestResourceNames())
        {
            Debug.WriteLine(name);
        }
        
        var _apiHost = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices(services => { })
                    .Configure(app =>
                    {
                        app.UseRouting();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapGet("/", async context =>
                            {
                                await context.Response.WriteAsync("WPF REST API Running!");
                            });

                            endpoints.MapGet("/api/data", async context =>
                            {
                                await context.Response.WriteAsync("{\"message\": \"Hello from WPF API\"}");
                            });
                            
                            endpoints.MapGet("/overlay/inputs", () =>
                                {
                                    var assembly = typeof(StartAPI).Assembly;
                                    var resourceName = "RaceOverlay.API.Overlays.Inputs.Inputs.html";

                                    using var stream = assembly.GetManifestResourceStream(resourceName);
                                    if (stream == null)
                                    {
                                        return Results.NotFound("Overlay file not found");
                                    }

                                    using var reader = new StreamReader(stream);
                                    var htmlContent = reader.ReadToEnd();
                                    return Results.Content(htmlContent, "text/html");
                                })
                                .WithName("GetInputsOverlay");

                            endpoints.MapGet("/overlay/inputs/data", () =>
                                {
                                    Debug.WriteLine("GetInputsOverlayData");
                                    InputsModel data = new InputsModel();
                                    return Results.Ok(data);
                                })
                                .WithName("GetInputsOverlayData");
                        });
                        
                    })
                    .UseUrls("http://localhost:5480"); // Change port if needed
            })
            .Build();

        _apiHost.StartAsync();
        return _apiHost;
    }
    
}