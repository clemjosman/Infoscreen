using System;

namespace Infoscreens.Common.Enumerations.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class SasTokenAttribute : Attribute
    {
        public SasTokenAttribute(string endPoint, string policyName, string policyKey, int minutesOfValidity)
        {
            EndPoint = endPoint;
            PolicyName = policyName;
            PolicyKey = policyKey;
            MinutesOfValidity = minutesOfValidity;
        }

        public string EndPoint { get; }
        public string PolicyName { get; }
        public string PolicyKey { get; }
        public int MinutesOfValidity { get; }
    }
}
