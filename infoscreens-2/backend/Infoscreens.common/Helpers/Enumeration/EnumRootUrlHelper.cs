using Infoscreens.Common.Enumerations.Attributes;

namespace Infoscreens.Common.Helpers.Enumerations
{
    public static class EnumRootUrlHelper
    {
        public static string GetRootUrl(this object enumVal)
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            if (memInfo == null || memInfo.Length == 0)
                return enumVal.ToString();

            var attributes = memInfo[0].GetCustomAttributes(typeof(RootUrlAttribute), false);
            return (attributes.Length > 0) ? ((RootUrlAttribute)attributes[0]).RootUrl : enumVal.ToString();
        }
    }
}