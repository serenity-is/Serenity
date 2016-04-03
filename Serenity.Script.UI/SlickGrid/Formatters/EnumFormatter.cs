using jQueryApi;
using System;

namespace Serenity
{
    public class EnumFormatter : ISlickFormatter
    {
        public EnumFormatter()
        {
        }

        [Option]
        public string EnumKey { get; set; }

        public string Format(SlickFormatterContext ctx)
        {
            return Format(EnumTypeRegistry.Get(EnumKey), ctx.Value);
        }

        public static string Format(Type enumType, object value)
        {
            if (!Script.IsValue(value))
                return "";

            string name;
            try
            {
                name = System.Enum.ToString(enumType, value.As<System.Enum>());
            }
            catch (ArgumentException)
            {
                name = value.ToString();
            }

            var enumKeyAttr = enumType.GetCustomAttributes(typeof(EnumKeyAttribute), false);
            var enumKey = enumKeyAttr.Length > 0 ? ((EnumKeyAttribute)enumKeyAttr[0]).Value : enumType.FullName;

            return GetText(enumKey, name);
        }

        public static string GetText(string enumKey, string name)
        {
            return Q.HtmlEncode(Q.TryGetText("Enums." + enumKey + "." + name) ?? name);
        }

        public static string GetText<TEnum>(TEnum? value)
            where TEnum: struct
        {
            if (!Script.IsValue(value))
                return "";

            return Format(typeof(TEnum), value.Value);
        }

        public static string GetName<TEnum>(TEnum? value)
            where TEnum : struct
        {
            if (!Script.IsValue(value))
                return "";

            return Enum.ToString(typeof(TEnum), value.Value.As<Enum>());
        }
    }
}