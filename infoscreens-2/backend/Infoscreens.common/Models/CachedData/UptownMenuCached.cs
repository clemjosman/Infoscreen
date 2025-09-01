namespace Infoscreens.Common.Models.CachedData
{
    public class UptownMenuCached
    {
        public string Base64Pdf { get; set; }

        public UptownMenuCached() { }

        public UptownMenuCached(string base64Pdf)
        {
            Base64Pdf = base64Pdf;
        }
    }
}
