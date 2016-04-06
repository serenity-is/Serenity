using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Serenity
{
    public static class ReflectionUtils
    {
        public static object GetPropertyValue(object o, string property)
        {
            var d = (dynamic)o;
            var getter = d["get_" + property];
            if (!Script.IsUndefined(getter))
                return ((dynamic)getter).apply(o);

            var camelCase = MakeCamelCase(property);
            getter = d["get_" + camelCase];
            if (!Script.IsUndefined(getter))
                return ((dynamic)getter).apply(o);

            return d[camelCase];
        }

        public static void SetPropertyValue(object o, string property, object value)
        {
            var d = (dynamic)o;
            var setter = d["set_" + property];
            if (!Script.IsUndefined(setter))
            {
                ((dynamic)setter).apply(o, new[] { value });
                return;
            }

            var camelCase = MakeCamelCase(property);
            setter = d["set_" + camelCase];
            if (!Script.IsUndefined(setter))
            {
                ((dynamic)setter).apply(o, new[] { value });
                return;
            }

            d[camelCase] = value;
        }

        public static string MakeCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            if (s == "ID")
                return "id";

            bool hasNonUppercase = false;
            int numUppercaseChars = 0;
            for (int index = 0; index < s.Length; index++)
            {
                if (s.CharCodeAt(index) >= 'A' && s.CharCodeAt(index) <= 'Z')
                {
                    numUppercaseChars++;
                }
                else
                {
                    hasNonUppercase = true;
                    break;
                }
            }

            if ((!hasNonUppercase && s.Length != 1) || numUppercaseChars == 0)
                return s;
            else if (numUppercaseChars > 1)
                return s.Substring(0, numUppercaseChars - 1).ToLowerCase() + s.Substr(numUppercaseChars - 1);
            else if (s.Length == 1)
                return s.ToLowerCase();
            else
                return s.Substr(0, 1).ToLowerCase() + s.Substr(1);
        }
    }
}