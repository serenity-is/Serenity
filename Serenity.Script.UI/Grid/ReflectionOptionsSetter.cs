using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
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

            var propList = props.Where(x => x.CanWrite &&
                    (x.GetCustomAttributes(typeof(OptionAttribute)).Length > 0 ||
                        x.GetCustomAttributes(typeof(DisplayNameAttribute)).Length > 0));

            var propByName = new JsDictionary<string, PropertyInfo>();
            foreach (var k in propList)
                propByName[ReflectionUtils.MakeCamelCase(k.Name)] = k;

            foreach (var k in options.Keys)
            {
                PropertyInfo p = propByName[ReflectionUtils.MakeCamelCase(k)];
                if (p != null)
                    p.SetValue(target, options[k]);
            }
        }
    }
}