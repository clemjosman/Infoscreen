namespace Infoscreens.Common.Models.ApiResponse
{
    public class MSB_Node
    {
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
        public string Tags { get; set; }
        public string Platform { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public bool? Tracking { get; set; }
        public string Iccid { get; set; }
        public string ManufactureId { get; set; }
        public string Imei { get; set; }
        public bool AllowSend { get; set; }
        public string TimeZone { get; set; }
        public int RetentionPeriod { get; set; }
    }
}
