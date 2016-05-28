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

            var type = target.GetType();

            if (type == typeof(Object))
                return;

            var propByName = (JsDictionary<string, PropertyInfo>)(type.As<dynamic>().__propByName);
            var fieldByName = (JsDictionary<string, FieldInfo>)(type.As<dynamic>().__fieldByName);

            if (propByName == null)
            {
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                var propList = props.Where(x => x.CanWrite &&
                        (x.GetCustomAttributes(typeof(OptionAttribute)).Length > 0 ||
                            x.GetCustomAttributes(typeof(DisplayNameAttribute)).Length > 0));

                propByName = new JsDictionary<string, PropertyInfo>();
                foreach (var k in propList)
                    propByName[ReflectionUtils.MakeCamelCase(k.Name)] = k;

                type.As<dynamic>().__propByName = propByName;
            }

            if (fieldByName == null)
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                var fieldList = fields.Where(x =>
                        (x.GetCustomAttributes(typeof(OptionAttribute)).Length > 0 ||
                            x.GetCustomAttributes(typeof(DisplayNameAttribute)).Length > 0));

                fieldByName = new JsDictionary<string, FieldInfo>();
                foreach (var k in fieldList)
                    fieldByName[ReflectionUtils.MakeCamelCase(k.Name)] = k;

                type.As<dynamic>().__fieldByName = fieldByName;
            }

            foreach (var k in options.Keys)
            {
                var v = options[k];
                var cc = ReflectionUtils.MakeCamelCase(k);
                PropertyInfo p = propByName[cc] ?? propByName[k];
                if (p != null)
                    p.SetValue(target, v);
                else
                {
                    FieldInfo f = fieldByName[cc] ?? fieldByName[k];
                    if (f != null)
                        f.SetValue(target, v);
                }
            }
        }
    }
}