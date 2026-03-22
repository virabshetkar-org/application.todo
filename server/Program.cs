using System.Diagnostics;

namespace Service.Todo;

public class Program
{
    public static void Main(string[] args)
    {
        if (Environment.GetEnvironmentVariable("WAIT_FOR_DEBUGGER") == "1")
        {
            Console.WriteLine("Waiting for debugger to attach...");
            while (!Debugger.IsAttached)
            {
                Thread.Sleep(1000);
            }
            Console.WriteLine("Debugger attached!");
        }

        var builder = WebApplication.CreateBuilder(args);

        var useRemoteConfig = Environment.GetEnvironmentVariable("USE_REMOTE_CONFIG");
        var remoteConfigUrl = Environment.GetEnvironmentVariable("REMOTE_CONFIG_URL");

        if (useRemoteConfig == "true" && remoteConfigUrl is not null)
        {
            Console.WriteLine("Adding Remote Config...");
            builder.Configuration.Sources.Clear();
            builder.Configuration.AddEnvironmentVariables();
            builder.Configuration.AddRemoteConfig(remoteConfigUrl);
        }

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
