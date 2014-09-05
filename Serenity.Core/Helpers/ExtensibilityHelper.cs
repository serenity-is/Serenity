using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Extensibility
{
    public static class ExtensibilityHelper
    {
        private static Assembly[] selfAssemblies;

        public static Assembly[] SelfAssemblies
        {
            get { return selfAssemblies; }
            set 
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                selfAssemblies = value; 
            }
        }

        static ExtensibilityHelper()
        {
            selfAssemblies = new Assembly[] { typeof(ExtensibilityHelper).Assembly };
        }

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

        public static void RunClassConstructor(Type type)
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }
    }
}
