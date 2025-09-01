using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Infoscreens.Common.Helpers.Enumerations
{
    public static class EnumMemberParamHelper
    {
        public static string GetEnumMemberAttrValue(this object enumVal)
        {
            if (enumVal == null) return null;

            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attr = memInfo[0].GetCustomAttributes(false).OfType<EnumMemberAttribute>().FirstOrDefault();
            if (attr != null)
            {
                return attr.Value;
            }

            return null;
        }

        public static T ToEnum<T>(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return default;

            var enumType = Nullable.GetUnderlyingType(typeof(T));

            if (string.IsNullOrWhiteSpace(str))
            {
                if (enumType != null) return default; // If nullable, return null
                else throw new Exception("Requested EnumMemberAttribute cannot be null or empty, received: " + str);
            }

            if (enumType == null) enumType = typeof(T);

            foreach (var name in Enum.GetNames(enumType))
            {
                var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
                if (enumMemberAttribute.Value == str) return (T)Enum.Parse(enumType, name);
            }
            throw new Exception("No matching enum member was found with an EnumMember value of "+str);
        }
    }
}