using System.Diagnostics;
using System.IO;
using System.Windows;
using API_test.Overlays.Inputs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RaceOverlay.API.Overlays.EnergyInfo;
using RaceOverlay.API.Overlays.SetupHider;

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
                            //
                            // Test endpoint
                            //
                            endpoints.MapGet("/", async context =>
                            {
                                await context.Response.WriteAsync("WPF REST API Running!");
                            });

                            endpoints.MapGet("/api/data", async context =>
                            {
                                await context.Response.WriteAsync("{\"message\": \"Hello from WPF API\"}");
                            });
                            
                            
                            //
                            // Inputs
                            //
                            
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
                            
                            
                            //
                            // Setuphider
                            //
                            
                            endpoints.MapGet("/overlay/setup_hider", () =>
                                {
                                    var assembly = typeof(StartAPI).Assembly;
                                    var resourceName = "RaceOverlay.API.Overlays.SetupHider.SetupHider.html";

                                    using var stream = assembly.GetManifestResourceStream(resourceName);
                                    if (stream == null)
                                    {
                                        return Results.NotFound("Overlay file not found");
                                    }

                                    using var reader = new StreamReader(stream);
                                    var htmlContent = reader.ReadToEnd();
                                    return Results.Content(htmlContent, "text/html");
                                })
                                .WithName("GetSetupHiderOverlay");

                            endpoints.MapGet("/overlay/setup_hider/data", () =>
                                {
                                    Debug.WriteLine("GetSetupHiderOverlayData");
                                    SetupHiderModel data = new SetupHiderModel();
                                    return Results.Ok(data);
                                })
                                .WithName("GetSetupHiderOverlayData");
                            
                            endpoints.MapGet("/overlay/setup_hider/image", () =>
                            {
                                Debug.WriteLine("GetSetupHiderImage");
                                
                                var imagePath = Path.Combine(App.AppDataPath, "SetupHider.jpg");
                                ImageClass data = new ImageClass(ConvertImageToBase64(imagePath));
                                return Results.Ok(data);
                            }).WithName("GetSetupHiderImage");
                            
                            
                            //
                            // Energy Info
                            //
                            
                            endpoints.MapGet("/overlay/energy_info", () =>
                                {
                                    var assembly = typeof(StartAPI).Assembly;
                                    var resourceName = "RaceOverlay.API.Overlays.EnergyInfo.EnergyInfo.html";

                                    using var stream = assembly.GetManifestResourceStream(resourceName);
                                    if (stream == null)
                                    {
                                        return Results.NotFound("Overlay file not found");
                                    }

                                    using var reader = new StreamReader(stream);
                                    var htmlContent = reader.ReadToEnd();
                                    return Results.Content(htmlContent, "text/html");
                                })
                                .WithName("GetEnergyInfoOverlay");

                            endpoints.MapGet("/overlay/energy_info/data", () =>
                            {
                                Debug.WriteLine("GetEnergyInfoOverlayData");
                                EnergyInfoModel data = new EnergyInfoModel();
                                return Results.Ok(data);
                            });
                            
                        });
                    })
                    .UseUrls("http://localhost:5480");
            })
            .Build();

        _apiHost.StartAsync();
        return _apiHost;
    }
    
    private static string ConvertImageToBase64(string imagePath)
    {
        Debug.WriteLine($"Looking for image at: {imagePath}");
        if (!File.Exists(imagePath))
        {
            Debug.WriteLine("Image file not found!");
            return string.Empty;
        }

        try
        {
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            string base64String = Convert.ToBase64String(imageBytes);
            Debug.WriteLine("Image successfully converted to Base64.");
            return $"data:image/jpeg;base64,{base64String}";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error reading image file: {ex.Message}");
            return string.Empty;
        }
    }
    
}