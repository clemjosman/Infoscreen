namespace Infoscreens.Common.Models.ConfigObjects.IoT_Hub_DeviceTwin
{
    public class DeviceTwin
    {
        public string Etag { get; set; }
        public string DeviceId { get; set; }
        public string DeviceEtag { get; set; }
        public int Version { get; set; }
        //public object Tags { get; set; }
        public DeviceTwinProperties Properties { get; set; }
        public DeviceTwinCapabilities Capabilities { get; set; }
        public string Status { get; set; }
        public string StatusUpdateTime { get; set; }
        public string LastActivityTime { get; set; }
        public string ConnectionState { get; set; }
        public int CloudToDeviceMessageCount { get; set; }
        public string SuthenticationType { get; set; }
        //public object X509Thumbprint { get; set; }
    }
}
