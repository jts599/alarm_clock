using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using AlarmClock.Backend.Services;
using AlarmClock.Backend.Services.Stubs;
using AlarmClock.Backend.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Bind configuration sections
builder.Services.Configure<WeatherConfiguration>(
    builder.Configuration.GetSection("Weather"));
builder.Services.Configure<RunConfiguration>(
    builder.Configuration.GetSection("RunConfiguration"));
builder.Services.Configure<LlmConfiguration>(
    builder.Configuration.GetSection("Llm"));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get configuration to determine which LIFX service to use
var runConfig = builder.Configuration.GetSection("RunConfiguration").Get<RunConfiguration>();

// Register LIFX service conditionally based on configuration
if (runConfig?.StubLifx == true)
{
    builder.Services.AddSingleton<ILifxService, StubLifxService>();
}
else
{
    builder.Services.AddSingleton<ILifxService, LifxService>();
}

// Register LLM service conditionally based on configuration
if (runConfig?.StubLifx == true) // Use same flag for simplicity, or add separate StubLlm flag
{
    builder.Services.AddSingleton<ILlmService, StubLlmService>();
}
else
{
    builder.Services.AddSingleton<ILlmService, LlmService>();
}

// Configure default alarm settings for ConfigurableColorPickingService
var defaultAlarmParameters = new ConfigurableColorPickingServiceConstructionParameters
{
    AlarmTime = new TimeOnly(6, 30), // 6:30 AM
    TransitionMinutes = 90, // 1.5 hours sunrise simulation
    HoldOnMinutes = 60, // Hold on for 1 hour after sunrise
    ActiveDays = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday }
};

// Register ConfigurableColorPickingService as the base service
builder.Services.AddTransient<ConfigurableColorPickingService>(provider =>
    new ConfigurableColorPickingService(defaultAlarmParameters));

// Register OverrideableColorPickingService with ConfigurableColorPickingService as the base
builder.Services.AddTransient<OverrideableColorPickingService>(provider =>
{
    var baseColorPicker = provider.GetRequiredService<ConfigurableColorPickingService>();
    return new OverrideableColorPickingService(baseColorPicker);
});

// Register the main ICompositeColorPickingService interface to use OverrideableColorPickingService
builder.Services.AddTransient<ICompositeColorPickingService>(provider =>
    provider.GetRequiredService<OverrideableColorPickingService>());

// Keep IColorPickingService registration for backward compatibility if needed
builder.Services.AddTransient<IColorPickingService>(provider =>
    provider.GetRequiredService<OverrideableColorPickingService>());

// Register LightStateService as both the interface and the hosted service
builder.Services.AddSingleton<LightStateService>();
builder.Services.AddSingleton<ILightStateService>(provider => provider.GetService<LightStateService>());
builder.Services.AddHostedService<LightStateService>(provider => provider.GetService<LightStateService>());

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