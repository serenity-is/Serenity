#if COREFX
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace System
{
    public static class NetCoreExtensions
    {
        public static Assembly GetAssembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }

        public static Type GetBaseType(this Type type)
        {
            return type.GetTypeInfo().BaseType;
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this Type type, bool inherit = true)
            where TAttribute: System.Attribute
        {
            return type.GetTypeInfo().GetCustomAttribute<TAttribute>(inherit);
        }

        public static Attribute[] GetCustomAttributes(this Type type, bool inherit = true)
        {
            return type.GetTypeInfo().GetCustomAttributes(inherit).ToArray();
        }

        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this Type type, bool inherit = true)
             where TAttribute : System.Attribute
        {
            return type.GetTypeInfo().GetCustomAttributes<TAttribute>(inherit);
        }

        public static Attribute[] GetCustomAttributes(this Type type, Type attributeType, bool inherit = true)
        {
            return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit).ToArray();
        }

        public static bool GetIsAbstract(this Type type)
        {
            return type.GetTypeInfo().IsAbstract;
        }

        public static bool GetIsEnum(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }

        public static bool GetContainsGenericParameters(this Type type)
        {
            return type.GetTypeInfo().ContainsGenericParameters;
        }

        public static bool GetIsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }

        public static bool GetIsClass(this Type type)
        {
            return type.GetTypeInfo().IsClass;
        }

        public static bool GetIsInterface(this Type type)
        {
            return type.GetTypeInfo().IsInterface;
        }

        public static bool GetIsGenericTypeDefinition(this Type type)
        {
            return type.GetTypeInfo().IsGenericTypeDefinition;
        }

        public static bool GetIsPublic(this Type type)
        {
            return type.GetTypeInfo().IsPublic;
        }

        public static bool IsSubclassOf(this Type type, Type other)
        {
            return type.GetTypeInfo().IsSubclassOf(other);
        }

        public static bool GetIsPrimitive(this Type type)
        {
            return type.GetTypeInfo().IsPrimitive;
        }
    }

    public class AppDomain
    {
        public static AppDomain CurrentDomain { get; private set; }

        static AppDomain()
        {
            CurrentDomain = new AppDomain();
        }

        public Assembly[] GetAssemblies()
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var library in dependencies)
            {
                if (IsCandidateCompilationLibrary(library))
                {
                    try
                    {
                        var assembly = Assembly.Load(new AssemblyName(library.Name));
                        assemblies.Add(assembly);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return assemblies.ToArray();
        }

        private static bool IsCandidateCompilationLibrary(RuntimeLibrary compilationLibrary)
        {
            return compilationLibrary.Name.StartsWith("Serenity.")
                || compilationLibrary.Dependencies.Any(d => d.Name.StartsWith("Serenity."));
        }
    }
}

namespace System.Configuration
{
    public static class ConfigurationManager
    {
        public static readonly AppSettingsAccessor AppSettings = new AppSettingsAccessor();
        public class AppSettingsAccessor
        {
            public string this[string key]
            {
                get
                {
                    return Serenity.Dependency.Resolve<IConfiguration>().GetSection("AppSettings")[key];
                }
            }
        }
    }

}
#endif