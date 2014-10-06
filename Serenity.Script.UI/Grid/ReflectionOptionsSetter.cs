using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Serenity.ComponentModel;
using System.ComponentModel;

namespace Serenity
{
    public static class ReflectionOptionsSetter
    {
        public static void Set(object target, JsDictionary<string, object> options)
        {
            if (options == null)
                return;

            var props = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var propByName = props.Where(x => x.CanWrite &&
                    (x.GetCustomAttributes(typeof(OptionAttribute)).Length > 0 ||
                        x.GetCustomAttributes(typeof(DisplayNameAttribute)).Length > 0))
                .ToDictionary(x => ReflectionUtils.MakeCamelCase(x.Name));

            foreach (var k in options.Keys)
            {
                PropertyInfo p;
                if (propByName.TryGetValue(ReflectionUtils.MakeCamelCase(k), out p))
                    p.SetValue(target, options[k]);
            }
        }
    }
}