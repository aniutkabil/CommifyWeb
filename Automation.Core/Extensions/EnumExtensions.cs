using System.Reflection;
using System.ComponentModel;

namespace Automation.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string? GetDescriptionOrValue(this Enum en)
        {
            if (en == null)
            {
                return null;
            }

            var type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return en.ToString();
        }

        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null)!;
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null)!;
                }
            }

            throw new ArgumentException($"There is no such enum by type {type} value with description: {description}");
        }

        public static string GetEnumDescriptionFromInt<TEnum>(int? value) where TEnum : Enum
        {
            var enumValue = (TEnum)Enum.ToObject(typeof(TEnum), value);

            var member = typeof(TEnum).GetMember(enumValue.ToString()).FirstOrDefault();
            if (member == null) return enumValue.ToString();

            var descriptionAttr = member.GetCustomAttribute<DescriptionAttribute>();
            return descriptionAttr?.Description ?? enumValue.ToString();
        }

        public static string GetDescriptionFromValue<TEnum>(this int value) where TEnum : Enum
        {
            return GetEnumDescriptionFromInt<TEnum>(value);
        }

        public static string GetDescriptionFromValue<TEnum>(this int? value) where TEnum : Enum
        {
            return GetEnumDescriptionFromInt<TEnum>(value);
        }
    }
}
