using System;

namespace Infoscreens.Common.Enumerations.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public class QueryParamAttribute : Attribute
    {
        public QueryParamAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public string Value { get; }
    }
}
