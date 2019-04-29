using System;
using System.Collections.Generic;
#if !COREFX
using System.IO;
#endif
using System.Linq;
using System.Reflection;

namespace Serenity.Extensibility
{
    /// <summary>
    /// Contains helper functions for extensibility through reflection.
    /// </summary>
    public static class ExtensibilityHelper
    {
        private static Assembly[] selfAssemblies;

        /// <summary>
        /// Gets or sets the self assemblies. These are assemblies that has
        /// reference to Serenity libraries, and those that should be scanned
        /// during code generation etc.
        /// </summary>
        /// <value>
        /// The self assemblies.
        /// </value>
        /// <exception cref="ArgumentNullException">value</exception>
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

        /// <summary>
        /// Gets the types with interface.
        /// </summary>
        /// <param name="intf">The intf.</param>
        /// <param name="assemblies">The assemblies. If null self assemblies are used.</param>
        /// <returns>Types with given interface</returns>
        public static IEnumerable<Type> GetTypesWithInterface(Type intf, Assembly[] assemblies = null)
        {
            foreach (var assembly in assemblies ?? SelfAssemblies)
            {
                foreach (var type in assembly.GetTypes())
                    if (!type.IsInterface &&
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
        /// <summary>
        /// Enumerates the directory for assemblies.
        /// </summary>
        /// <param name="assemblies">The dictionary to add loaded assemblies to.</param>
        /// <param name="path">The path.</param>
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

        /// <summary>
        /// Runs the class constructor (static constructor) if any.
        /// </summary>
        /// <param name="type">The type.</param>
        public static void RunClassConstructor(Type type)
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }
    }
}