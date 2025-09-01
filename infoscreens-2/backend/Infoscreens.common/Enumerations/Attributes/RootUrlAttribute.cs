using System;

namespace Infoscreens.Common.Enumerations.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class RootUrlAttribute : Attribute
    {
        public RootUrlAttribute(string rootUrl)
        {
            RootUrl = rootUrl;
        }

        public string RootUrl { get; }
    }
}
