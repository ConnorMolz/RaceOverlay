using System.Windows;
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
                        });
                    })
                    .UseUrls("http://localhost:5480"); // Change port if needed
            })
            .Build();

        _apiHost.StartAsync();
        return _apiHost;
    }
    
}