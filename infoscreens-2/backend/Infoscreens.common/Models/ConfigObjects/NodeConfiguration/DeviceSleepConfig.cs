using Newtonsoft.Json;

namespace Infoscreens.Common.Models.Configs
{
    public class DeviceSleepConfig
    {

        [JsonProperty(Required = Required.Always)]
        public SleepConfig Daily { get; set; }

        [JsonProperty(Required = Required.Always)]
        public SleepConfig Weekend { get; set; }

        public DeviceSleepConfig() : this(null, null) {}

        public DeviceSleepConfig(SleepConfig daily, SleepConfig weekend)
        {
            Daily = daily ?? new SleepConfig(null, null);
            Weekend = weekend ?? new SleepConfig(null, null);
        }
    }

    public class SleepConfig
    {
        [JsonProperty(Required = Required.AllowNull)]
        public string StartTime { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public int? Duration { get; set; }

        public SleepConfig() : this(null, null) {}

        public SleepConfig(string startTime, int? duration)
        {
            StartTime = string.IsNullOrWhiteSpace(startTime) ? null : startTime;
            Duration = duration ?? null;
        }
    }
}
