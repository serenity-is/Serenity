using System;
using System.Collections.Generic;
#if !COREFX
using System.IO;
#endif
using System.Linq;
using System.Reflection;

namespace Serenity.Extensibility
{
    public static class ExtensibilityHelper
    {
        private static Assembly[] selfAssemblies;

        public static Assembly[] SelfAssemblies
        {
            get
            {
                if (selfAssemblies == null)
                    selfAssemblies = DetermineSelfAssemblies();

                return selfAssemblies;
            }
            set 
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                selfAssemblies = value; 
            }
        }

        public static IEnumerable<Type> GetTypesWithInterface(Type intf, Assembly[] assemblies = null)
        {
            foreach (var assembly in assemblies ?? SelfAssemblies)
            {
                foreach (var type in assembly.GetTypes())
                    if (!type.GetIsInterface() &&
                        intf.IsAssignableFrom(type))
                        yield return type;
            }
        }

        private static bool ReferencesSerenity(Assembly assembly)
        { 
            return assembly.FullName.Contains("Serenity") ||
                assembly.GetReferencedAssemblies().Any(a => a.Name.Contains("Serenity"));
        }

#if !COREFX
        private static void EnumerateDirectory(Dictionary<string, Assembly> assemblies, string path)
        {
            foreach (var filename in Directory.GetFiles(path, "*.dll"))
            try
            {
                if (assemblies.ContainsKey(Path.GetFileNameWithoutExtension(Path.GetFileName(filename))))
                    continue;

                var asm = Assembly.LoadFrom(filename);
                var name = asm.GetName().Name;
                if (!assemblies.ContainsKey(name) && ReferencesSerenity(asm))
                    assemblies.Add(name, asm);
            }
            catch (Exception)
            {
            }
        }
#endif

        private static Assembly[] DetermineSelfAssemblies()
        {
            var assemblies = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = asm.GetName().Name;
                if (!assemblies.ContainsKey(name) && ReferencesSerenity(asm))
                {
                    assemblies.Add(name, asm);

                    foreach (var reference in asm.GetReferencedAssemblies())
                    {
                        name = reference.Name;
                        if (!assemblies.ContainsKey(name))
                        {
                            try
                            {
                                var refasm = Assembly.Load(reference);
                                if (ReferencesSerenity(refasm))
                                    assemblies.Add(name, refasm);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }

#if !COREFX
            var asmPath = Path.GetDirectoryName(typeof(ExtensibilityHelper).Assembly.Location);
            EnumerateDirectory(assemblies, asmPath);
#endif

            return Reflection.AssemblySorter.Sort(assemblies.Values).ToArray();
        }


        public static void RunClassConstructor(Type type)
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }
    }
}
