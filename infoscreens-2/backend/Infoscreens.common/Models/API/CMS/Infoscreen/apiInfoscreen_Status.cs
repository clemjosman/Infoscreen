using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.CachedData;
using Infoscreens.Common.Models.EntityFramework.CMS;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiInfoscreen_Status : apiInfoscreen_Light
    {
        public InfoscreenNodeStatusCached Status {get; set;}

        public apiInfoscreen_Status(Infoscreen infoscreen, InfoscreenNodeStatusCached status, IDatabaseRepository databaseRepository): base(infoscreen, databaseRepository)
        {
            Status = status;
        }
    }
}
