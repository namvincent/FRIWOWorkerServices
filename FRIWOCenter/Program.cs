using FRIWOCenter.Components;
using FRIWOCenter.Data.TRACE;
using FRIWOCenter.DBServices;
using FRIWOCenter.DBServices.SerialPorts;
using FRIWOCenter.Shared;
using MatBlazor;
using FRIWOCenter.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using FRIWOCenter.Hubs;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalRCore();
builder.Services.AddHttpClient();
builder.Services.AddDevExpressBlazor();
builder.Services.AddMatBlazor();
builder.Services.AddMatToaster(config =>
{
    config.Position = MatToastPosition.BottomRight;
    config.PreventDuplicates = true;
    config.NewestOnTop = true;
    config.ShowCloseButton = true;
    config.MaximumOpacity = 95;
    config.VisibleStateDuration = 1000;
});
//builder.Services.AddMvc();
//builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
//builder.Services.Configure<RequestLocalizationOptions>(options => {
//    options.DefaultRequestCulture = new RequestCulture("de");
//    var supportedCultures = new List<CultureInfo>() { new CultureInfo("es") };
//    options.SupportedCultures = supportedCultures;
//    options.SupportedUICultures = supportedCultures;
//});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:5002", "http://localhost:5000", "http://fvn-s-ws01.friwo.local:85");
        });
});

builder.Services.Configure<DevExpress.Blazor.Configuration.GlobalOptions>(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
});
#region InjectionServices

builder.Services.AddScoped<BusinessReport>();
builder.Services.AddScoped<RoutingResults>();
builder.Services.AddScoped<StationResult>();
builder.Services.AddScoped<LinkInfo>();
builder.Services.AddScoped<ProcessLock>();
builder.Services.AddScoped<V_ROUTING_BY_PART_NO>();
builder.Services.AddSingleton<SerialService>();
builder.Services.AddSingleton<MainLayout>();
//builder.Services.AddScoped<NavMenu>();
builder.Services.AddSingleton<Loading>();
//builder.Services.AddScoped<Zalo3rdAppInfo>();
//builder.Services.AddScoped<Zalo3rdAppClient>();
#endregion
#region Helper
// pager
//builder.Services.AddScoped<IPageHelper, PageHelper>();

//// filters
//builder.Services.AddScoped<IFilters, GridControls>();

//// query adapter (applies filter to contact request).
//builder.Services.AddScoped<GridQueryAdapter>();

//// service to communicate success on edit between pages
//builder.Services.AddScoped<EditSuccess>();
#endregion
#region DBServices
builder.Services.AddScoped<MonthlyOutputService>();
builder.Services.AddScoped<ShopOrderViewService>();
builder.Services.AddScoped<HusqvarnaServices>();
builder.Services.AddScoped<ProcessLockService>();
builder.Services.AddScoped<BusinessReportService>();
builder.Services.AddScoped<WIPAreaService>();
builder.Services.AddScoped<WIPScanService>();
#endregion

#region DBContext
#if DEBUG

builder.Services.AddDbContextFactory<TraceDbContext>(opt => opt.UseOracle(builder.Configuration.GetConnectionString("TraceConnectionPooling")).EnableSensitiveDataLogging());
#else
builder.Services.AddDbContextFactory<TraceDbContext>(opt => opt.UseOracle(builder.Configuration.GetConnectionString("TraceConnectionPooling")));
#endif
#endregion
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Host.UseWindowsService();
var app = builder.Build();
// Configure the HTTP request pipeline.
if(!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//app.UseRequestLocalization();

//app.UseHttpsRedirection();

app.UseCors();
app.UseStaticFiles();
app.UseRouting();
app.MapHub<ChatHub>("/chathub");
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
