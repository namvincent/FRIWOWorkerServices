using FRIWO.WorkerServices;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.File("C:/Temp/log.txt")
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .UseSerilog()
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        //services.AddHostedService<CallApi>();
    })
    .Build();
try
{
    Log.Information("Servives is started");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex.ToString());
}
finally
{
    Log.CloseAndFlush();
}


