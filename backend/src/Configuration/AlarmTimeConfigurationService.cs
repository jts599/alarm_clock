using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;

namespace AlarmClock.Backend.Configuration
{
    public interface IAlarmTimeConfigurationService
    {
        AlarmTimeConfiguration Get();
        Task SaveAsync(AlarmTimeConfiguration config);
    }

    public class AlarmTimeConfigurationService : IAlarmTimeConfigurationService
    {
        private readonly string _filePath;
    private readonly object _writeLock = new object();
    // Initialize to a default instance so this field is always non-null (option 2)
    private AlarmTimeConfiguration _current = new AlarmTimeConfiguration();

        public AlarmTimeConfigurationService(IWebHostEnvironment env, IOptionsMonitor<AlarmTimeConfiguration> options)
        {
            _filePath = Path.Combine(env.ContentRootPath, "AlarmTimeConfiguration.json");
            // Use the provided options value if available, otherwise keep the pre-initialized default
            _current = options.CurrentValue ?? _current;
        }

        public AlarmTimeConfiguration Get()
        {
            // Return a copy to avoid accidental mutation of the stored instance
            var c = _current;
            return new AlarmTimeConfiguration
            {
                StartTimeInMinutesSinceMidnight = c.StartTimeInMinutesSinceMidnight,
                TransitionDurationInMinutes = c.TransitionDurationInMinutes,
                StayOnTimeInMinutes = c.StayOnTimeInMinutes,
                ActiveDays = c.ActiveDays ?? Array.Empty<string>()
            };
        }

        public Task SaveAsync(AlarmTimeConfiguration config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            // Prepare the root object to match the file layout
            var root = new { AlarmTimeConfiguration = config };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(root, options);

            // Ensure atomic write
            var tmp = _filePath + ".tmp";

            lock (_writeLock)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath) ?? ".");
                File.WriteAllText(tmp, json);
                File.Move(tmp, _filePath, overwrite: true);
                // Update in-memory copy
                _current = new AlarmTimeConfiguration
                {
                    StartTimeInMinutesSinceMidnight = config.StartTimeInMinutesSinceMidnight,
                    TransitionDurationInMinutes = config.TransitionDurationInMinutes,
                    StayOnTimeInMinutes = config.StayOnTimeInMinutes,
                    ActiveDays = config.ActiveDays ?? Array.Empty<string>()
                };
            }

            return Task.CompletedTask;
        }
    }
}
