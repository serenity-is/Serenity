using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serenity.Web
{
    public class DistinctValuesRegistration
    {
        public static void RegisterDistinctValueScripts()
        {
            var assemblies = ExtensibilityHelper.SelfAssemblies;
            var list = new List<DistinctValuesEditorAttribute>();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    bool isRow = type.IsSubclassOf(typeof(Row));

                    if (!isRow &&
                        type.GetCustomAttribute<FormScriptAttribute>() == null &&
                        type.GetCustomAttribute<ColumnsScriptAttribute>() == null)
                    {
                        continue;
                    }

                    if (isRow &&
                        type.IsAbstract)
                        continue;

                    foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        var attr = property.GetCustomAttribute<DistinctValuesEditorAttribute>();
                        if (attr == null)
                            continue;

                        if (attr.RowType != null)
                        {
                            if (attr.RowType.IsInterface ||
                                attr.RowType.IsAbstract ||
                                !attr.RowType.IsSubclassOf(typeof(Row)))
                            {
                                throw new Exception("DistinctValuesEditor can't be used with type: " +
                                    attr.RowType.FullName + " as it is not a row type. This attribute is specified " +
                                    "on " + property.Name + " property of " + type.FullName);
                            }

                            attr.PropertyName = attr.PropertyName.IsEmptyOrNull() ? property.Name :
                                attr.PropertyName;
                        }
                        else
                        {
                            if (!isRow)
                            {
                                var basedOnRowAttr = type.GetCustomAttribute<BasedOnRowAttribute>();
                                if (basedOnRowAttr == null || basedOnRowAttr.RowType == null ||
                                    basedOnRowAttr.RowType.IsAbstract ||
                                    !basedOnRowAttr.RowType.IsSubclassOf(typeof(Row)))
                                {
                                    throw new Exception("Invalid usage of DistinctValuesEditor attribute on " +
                                        "property " + property.Name + " of " + type.FullName + ". " +
                                        "RowType has to be specified!");
                                }

                                attr.RowType = basedOnRowAttr.RowType;
                            }
                            else
                                attr.RowType = type;

                            attr.PropertyName = attr.PropertyName.IsEmptyOrNull() ? property.Name :
                                attr.PropertyName;
                        }

                        list.Add(attr);
                    }

                    var byRowProperty = list.ToLookup(x => new Tuple<Type, string>(x.RowType, x.PropertyName));

                    foreach (var key in byRowProperty)
                    {
                        var row = (Row)Activator.CreateInstance(key.Key.Item1);

                        var script = (LookupScript)Activator.CreateInstance(typeof(DistinctValuesScript<>)
                            .MakeGenericType(key.Key.Item1), new object[] { key.Key.Item2 });

                        script.LookupKey = "Distinct." + row.GetFields().LocalTextPrefix + "." +
                            key.Key.Item2;

                        var withPermission = key.FirstOrDefault(x => !string.IsNullOrEmpty(x.Permission));
                        if (withPermission != null)
                            script.Permission = withPermission.Permission;

                        var withExpiration = key.FirstOrDefault(x => x.Expiration != 0);
                        if (withExpiration != null)
                            script.Expiration = TimeSpan.FromSeconds(withExpiration.Expiration);

                        DynamicScriptManager.Register(script.ScriptName, script);
                    }
                }
            }
        }
    }
}
