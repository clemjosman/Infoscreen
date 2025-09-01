using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Helpers.Enumerations;
using Infoscreens.Common.Models.API.CMS;
using System.Collections.Generic;
using System;

namespace Infoscreens.Common.Models.Configs
{
    public class BackendConfig
    {
        public DataEndpointConfig DataEndpointConfig { get; set; } = null;

        // Methods
        public void Update(apiInfoscreen_ConfigUpdate update)
        {
            DataEndpointConfig = update.DataEndpointConfig;
        }

        public string GetCachedFileName(eApi api)
        {
            return DataEndpointConfig[api]?.CachedFileName;
        }

        public ICollection<string> GetCachedFileNames(eApi api)
        {
            return DataEndpointConfig[api]?.CachedFileNames;
        }
    }

    public class DataEndpointConfig
    {
        public CachedFilesEndpointConfig CustomJobOffer { get; set; }
        public CachedFilesEndpointConfig Ideabox { get; set; }
        // public CachedFilesEndpointConfig IotHub { get; set; } // Isn't used directly by the device - ignored here
        public CachedFilesEndpointConfig JobOffers { get; set; }
        public CachedFilesEndpointConfig Microservicebus { get; set; }
        public NewsInternalApiCacheConfig NewsInternal { get; set; }
        public NewsPublicApiCacheConfig NewsPublic { get; set; }
        public CachedFilesEndpointConfig OpenWeather { get; set; }
        public CachedFilesEndpointConfig PublicTransport { get; set; }
        public CachedFilesEndpointConfig Sociabble { get; set; }
        public TwentyMinApiCacheConfig TwentyMin { get; set; }
        public CachedFilesEndpointConfig Twitter { get; set; }
        public CachedFilesEndpointConfig University { get; set; }
        public CachedFilesEndpointConfig UptownArticle { get; set; }
        public CachedFilesEndpointConfig UptownEvent { get; set; }
        // public CachedFilesEndpointConfig UptownMenu { get; set; } // Doesn't provide specific configuration of selection between multiple files

        /// <summary>
        /// Gets config object dynamically.
        /// WARNING: Not all config have CachedFiles !
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public ICachedFilesEndpointConfig this[eApi api]
        {
            get { return GetConfigObject(api); }
        }

        public ICachedFilesEndpointConfig GetConfigObject(eApi api)
        {
            var propertyName = EnumMemberParamHelper.GetEnumMemberAttrValue(api);
            propertyName = string.Concat(propertyName[..1].ToUpper(), propertyName.AsSpan(1));
            return (ICachedFilesEndpointConfig) GetType().GetProperty(propertyName).GetValue(this, null);
        }
    }

    public interface ICachedFilesEndpointConfig {
        public string CachedFileName { get; set; }
        public ICollection<string> CachedFileNames { get; set; }
    }

    public class CachedFilesEndpointConfig : ICachedFilesEndpointConfig
    {
        public string CachedFileName { get; set; }
        public ICollection<string> CachedFileNames { get; set; }
    }

    public interface ILimitNewsDateAge {
        public int MaxNewsDateAge { get; set; }
    }

    public interface ILimitNewsCount {
        public int MaxNewsCount { get; set; }
    }

    public class TwentyMinApiCacheConfig : CachedFilesEndpointConfig, ILimitNewsDateAge {
        public int MaxNewsDateAge { get; set; }
    }

    public class NewsPublicApiCacheConfig : CachedFilesEndpointConfig, ILimitNewsCount {
        public int MaxNewsCount { get; set; }
    }

    public class NewsInternalApiCacheConfig : ILimitNewsCount
    {
        public int MaxNewsCount { get; set; }
    }
}
