using System;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.CachedData
{
    public class InfoscreenNodeStatusCached
    {

        // Node info
        public string Id { get; set; }
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public string MachineName { get; set; }
        public string Details { get; set; }
        public string PublicIp { get; set; }
        public bool Enabled { get; set; }
        public string OrganizationId { get; set; }
        public bool LockToMachine { get; set; }
        public string Protocol { get; set; }
        public string NpmVersion { get; set; }
        public bool Debug { get; set; }
        public int WebPort { get; set; }
        public string Mode { get; set; }
        public ICollection<string> Tags { get; set; }
        public string Platform { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public bool? Tracking { get; set; }
        public string Iccid { get; set; }
        public string ManufactureId { get; set; }
        public string Imei { get; set; }
        public bool AllowSend { get; set; }
        public string TimeZone { get; set; }
        public int RetentionPeriod{ get; set; }

        // Connection state
        public bool IsOnlineMsb { get; set; }
        public string DeviceId { get; set; }
        public string GenerationId { get; set; }
        public string Etag { get; set; }
        public string ConnectionState { get; set; }
        public string Status { get; set; }
        public string StatusReason { get; set; }
        public DateTimeOffset ConnectionStateUpdatedTime { get; set; }
        public DateTimeOffset StatusUpdatedTime { get; set; }
        public DateTimeOffset LastActivityTime { get; set; }
        public int CloudToDeviceMessageCount { get; set; }

        // Device State
        public string UiVersion { get; set; }
        public string DesiredUiVersion { get; set; }
        public string FirmwareVersion { get; set; }
        public string DesiredFirmwareVersion { get; set; }



        public InfoscreenNodeStatusCached(string id, string name, string connectionId, string machineName, string details, string publicIp, bool enabled, string organizationId, bool lockToMachine, string protocol, string npmVersion, bool debug, int webPort, string mode, ICollection<string> tags, string platform, string longitude, string latitude, bool? tracking, string iccid, string manufactureId, string imei, bool allowSend, string timeZone, int retentionPeriod, bool isOnlineMsb, string deviceId, string generationId, string etag, string connectionState, string status, string statusReason, DateTimeOffset connectionStateUpdatedTime, DateTimeOffset statusUpdatedTime, DateTimeOffset lastActivityTime, int cloudToDeviceMessageCount, string uiVersion, string desiredUiVersion, string firmwareVersion, string desiredFirmwareVersion)
        {
            // Node info
            Id = id;
            Name = name;
            ConnectionId = connectionId;
            MachineName = machineName;
            Details = details;
            PublicIp = publicIp;
            Enabled = enabled;
            OrganizationId = organizationId;
            LockToMachine = lockToMachine;
            Protocol = protocol;
            NpmVersion = npmVersion;
            Debug = debug;
            WebPort = webPort;
            Mode = mode;
            Tags = tags;
            Platform = platform;
            Longitude = longitude;
            Latitude = latitude;
            Tracking = tracking;
            Iccid = iccid;
            ManufactureId = manufactureId;
            Imei = imei;
            AllowSend = allowSend;
            TimeZone = timeZone;
            RetentionPeriod = retentionPeriod;

            // Connection state
            IsOnlineMsb = isOnlineMsb;
            DeviceId = deviceId;
            GenerationId = generationId;
            Etag = etag;
            ConnectionState = connectionState;
            Status = status;
            StatusReason = statusReason;
            ConnectionStateUpdatedTime = connectionStateUpdatedTime;
            StatusUpdatedTime = statusUpdatedTime;
            LastActivityTime = lastActivityTime;
            CloudToDeviceMessageCount = cloudToDeviceMessageCount;

            // Device State
            UiVersion = uiVersion;
            DesiredUiVersion = desiredUiVersion;
            FirmwareVersion = firmwareVersion;
            DesiredFirmwareVersion = desiredFirmwareVersion;
        }
    }
}
