using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using Serenity.Data;
using System.Reflection;
using Serenity.Web.PropertyEditor;
using Serenity.Web.FilterPanel;
using Serenity.Reflection;

namespace Serenity.Web
{
    /// <summary>
    ///   Helper class to write CSV files to HTTP context or another stream</summary>
    public class RegistrationHelper
    {
        public static void RegisterFormScripts(Assembly[] assemblies = null)
        {
            assemblies = assemblies ?? ExtensibilityHelper.SelfAssemblies;

            foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute<FormScriptAttribute>();
                    if (attr != null)
                    {
                        string key = attr.Key;
                        if (key.IsNullOrEmpty())
                        {
                            key = type.Name;
                            const string p = "Form";
                            if (key.EndsWith(p))
                                key = key.Substring(0, key.Length - p.Length);
                        }

                        new FormScript(key, type);
                    }
                }
        }
    }
}
