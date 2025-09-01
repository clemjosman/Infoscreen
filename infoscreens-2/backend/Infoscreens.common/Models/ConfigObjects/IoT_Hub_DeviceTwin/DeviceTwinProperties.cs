namespace Infoscreens.Common.Models.ConfigObjects.IoT_Hub_DeviceTwin
{
    public class DeviceTwinProperties
    {
        public object Desired { get; set; }
        public DeviceTwinPropertiesReported Reported { get; set; }
    }
}
