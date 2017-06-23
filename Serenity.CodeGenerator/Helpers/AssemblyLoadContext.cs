using System.Reflection;

#if !FXCORE
namespace Serenity.CodeGenerator
{
    /// <summary>
    ///  Compatibility class
    /// </summary>
    public abstract class AssemblyLoadContext
    {
        class DefaultImplementation: AssemblyLoadContext
        {
            protected override Assembly Load(AssemblyName assemblyName)
            {
               return Default.LoadFromAssemblyName(assemblyName);
            }
        }
        public static AssemblyLoadContext Default
        {
            get { return new DefaultImplementation(); }
        }

        protected abstract Assembly Load(AssemblyName assemblyName);
        

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public Assembly LoadFromAssemblyPath(string path)
        {
            return Assembly.LoadFrom(path);
        }


        // ReSharper disable once MemberCanBeMadeStatic.Global
        public Assembly LoadFromAssemblyName(AssemblyName assemblyName)
        {
            return Assembly.Load(assemblyName);
        }
    }
}
#endif