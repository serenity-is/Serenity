using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Extensibility;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Web
{
    public class LookupScriptRegistration
    {
        public static void RegisterLookupScripts()
        {
            var assemblies = ExtensibilityHelper.SelfAssemblies;
            var registeredType = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute<LookupScriptAttribute>();
                    if (attr == null)
                        continue;

                    LookupScript script;

                    if (typeof(Row).IsAssignableFrom(type))
                    {
                        if (attr.LookupType == null)
                            script = (LookupScript)Activator.CreateInstance(typeof(RowLookupScript<>).MakeGenericType(type));
                        else if (attr.LookupType.IsGenericType)
                            script = (LookupScript)Activator.CreateInstance(attr.LookupType.MakeGenericType(type));
                        else if (attr.LookupType.GetCustomAttribute<LookupScriptAttribute>() == null)
                            script = (LookupScript)Activator.CreateInstance(attr.LookupType);
                        else
                        {
                            // lookup script type already has a LookupScript attribute, 
                            // so it's dynamic script will be generated on itself
                            continue;
                        }
                    }
                    else if (!typeof(LookupScript).IsAssignableFrom(type) ||
                        type.IsAbstract)
                    {
                        throw new InvalidOperationException(String.Format("Type {0} can't be registered as a lookup script!", type.FullName));
                    }
                    else
                    {
                        script = (LookupScript)Activator.CreateInstance(type);
                    }

                    script.LookupKey = attr.Key;

                    Type otherType;
                    if (registeredType.TryGetValue(script.LookupKey, out otherType))
                    {
                        throw new InvalidOperationException(String.Format("Types {0} and {1} has the same lookup key (\"{2}\"). " +
                            "\r\n\r\nPlease remove LookupScript attribute from one of them or change the lookup key!",
                            type.FullName, otherType.FullName, script.LookupKey));
                    }

                    registeredType[script.LookupKey] = type;

                    if (attr.Permission != null)
                        script.Permission = attr.Permission;

                    if (attr.Expiration != 0)
                        script.Expiration = TimeSpan.FromSeconds(attr.Expiration);

                    DynamicScriptManager.Register(script.ScriptName, script);
                }
            }
        }
    }
}
