using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using SVS = AlarmClock.Backend.Services;
using AlarmClock.Backend.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configure logging with timestamps
builder.Logging.ClearProviders();
builder.Logging.AddSystemdConsole(options =>
{
    options.IncludeScopes = false;
    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
});

// Bind configuration sections
// Load runtime-updatable alarm configuration from a separate JSON file.
// This file is optional and will be reloaded when changed on disk.
builder.Configuration.AddJsonFile("AlarmTimeConfiguration.json", optional: true, reloadOnChange: true);
builder.Services.Configure<WeatherConfiguration>(
    builder.Configuration.GetSection("Weather"));
builder.Services.Configure<RunConfiguration>(
    builder.Configuration.GetSection("RunConfiguration"));
builder.Services.Configure<LlmConfiguration>(
    builder.Configuration.GetSection("Llm"));
// Bind the AlarmTimeConfiguration section (from AlarmTimeConfiguration.json)
builder.Services.Configure<AlarmTimeConfiguration>(
    builder.Configuration.GetSection("AlarmTimeConfiguration"));

// Register the AlarmTimeConfiguration persistence service
// Register the concrete type too so components that request AlarmTimeConfigurationService
// directly (rather than the interface) can be constructed by DI.
builder.Services.AddSingleton<AlarmTimeConfigurationService>();
builder.Services.AddSingleton<IAlarmTimeConfigurationService>(sp => sp.GetRequiredService<AlarmTimeConfigurationService>());

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get configuration to determine which LIFX service to use
var runConfig = builder.Configuration.GetSection("RunConfiguration").Get<RunConfiguration>();

// Register LIFX service conditionally based on configuration
if (runConfig?.StubLifx == true)
{
    builder.Services.AddSingleton<SVS.ILifxService, SVS.StubLifxService>();
}
else
{
    builder.Services.AddSingleton<SVS.ILifxService, SVS.LifxService>();
}

// Register Brightness service conditionally based on configuration
if (runConfig?.StubBrightness == true)
{
    builder.Services.AddSingleton<SVS.Brightness.IBrightnessService, SVS.Brightness.MockBrightnessService>();
}
else
{
    builder.Services.AddSingleton<SVS.Brightness.IBrightnessService, SVS.Brightness.RaspberryPiBrightnessService>();
}

// Register ConfigurableColorPickingServiceConstructionParameters to be created via DI
builder.Services.AddTransient<SVS.ConfigurableColorPickingServiceConstructionParameters>();

// Register ConfigurableColorPickingService as the base service and resolve its
// construction parameters from DI (so they come from AlarmTimeConfigurationService).
builder.Services.AddTransient<SVS.ConfigurableColorPickingService>(provider =>
{
    var parameters = provider.GetRequiredService<SVS.ConfigurableColorPickingServiceConstructionParameters>();
    return new SVS.ConfigurableColorPickingService(parameters);
});

// Register OverrideableColorPickingService with ConfigurableColorPickingService as the base
builder.Services.AddTransient<SVS.OverrideableColorPickingService>(provider =>
{
    var baseColorPicker = provider.GetRequiredService<SVS.ConfigurableColorPickingService>();
    return new SVS.OverrideableColorPickingService(baseColorPicker);
});

// Register the main ICompositeColorPickingService interface to use OverrideableColorPickingService
builder.Services.AddTransient<SVS.ICompositeColorPickingService>(provider =>
    provider.GetRequiredService<SVS.OverrideableColorPickingService>());

// Keep IColorPickingService registration for backward compatibility if needed
builder.Services.AddTransient<SVS.IColorPickingService>(provider =>
    provider.GetRequiredService<SVS.OverrideableColorPickingService>());
// Register LightStateService as both the interface and the hosted service
builder.Services.AddSingleton<SVS.LightStateService>();
builder.Services.AddSingleton<SVS.ILightStateService>(provider => provider.GetService<SVS.LightStateService>());
builder.Services.AddHostedService<SVS.LightStateService>(provider => provider.GetService<SVS.LightStateService>());

// Register state summary service (singleton) so controllers can get a cheap snapshot of state
builder.Services.AddSingleton<SVS.IStateSummaryService, SVS.StateSummaryService>();

// Add CORS policy for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/", () => Results.Ok(new { status = "Alarm Clock API is running", timestamp = DateTime.UtcNow }));

app.Run();