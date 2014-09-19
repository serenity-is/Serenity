using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Extensibility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Serenity.Web
{
    public class LookupScriptRegistration
    {
        public static void RegisterLookupScripts()
        {
            var assemblies = ExtensibilityHelper.SelfAssemblies;

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
                        script = (LookupScript)Activator.CreateInstance(typeof(RowLookupScript<>).MakeGenericType(type));
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

                    if (attr.Permission != null)
                    {
                        if (attr.Permission == "?" || attr.Permission == "")
                        {
                            script.Authorize = true;
                            script.Permission = null;
                        }
                        else if (attr.Permission == "*")
                        {
                            script.Authorize = false;
                            script.Permission = null;
                        }
                        else
                        {
                            script.Authorize = true;
                            script.Permission = attr.Permission;
                        }
                    }


                    DynamicScriptManager.Register(script.ScriptName, script);
                }
            }
        }
    }
}
