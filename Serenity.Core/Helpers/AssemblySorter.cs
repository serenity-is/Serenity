using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serenity.Reflection
{
    public class AssemblySorter
    {
        private class AssemblyItem
        {
            public Assembly Item { get; set; }
            public IList<AssemblyItem> Dependencies { get; set; }

            public AssemblyItem(Assembly item)
            {
                Item = item;
                Dependencies = new List<AssemblyItem>();
            }
        }

        public static IEnumerable<Assembly> Sort(IEnumerable<Assembly> assemblies)
        {
            var assemblyItems = assemblies.Select(a => new AssemblyItem(a)).ToList();

            foreach (var item in assemblyItems)
            {
                foreach (var reference in item.Item.GetReferencedAssemblies())
                {
                    var dependency = assemblyItems.SingleOrDefault(i => i.Item.FullName == reference.FullName);

                    if (dependency != null)
                        item.Dependencies.Add(dependency);
                }
            }

            return TSort(assemblyItems, i => i.Dependencies).Select(x => x.Item);
        }

        public static IEnumerable<T> TSort<T>(IEnumerable<T> source, 
            Func<T, IEnumerable<T>> dependencies, bool throwOnCycle = false)
        {
            var sorted = new List<T>();
            var visited = new HashSet<T>();

            foreach (var item in source)
                Visit(item, visited, sorted, dependencies, throwOnCycle);

            return sorted;
        }

        private static void Visit<T>(T item, HashSet<T> visited, List<T> sorted, 
            Func<T, IEnumerable<T>> dependencies, bool throwOnCycle)
        {
            if (!visited.Contains(item))
            {
                visited.Add(item);

                foreach (var dep in dependencies(item))
                    Visit(dep, visited, sorted, dependencies, throwOnCycle);

                sorted.Add(item);
            }
            else
            {
                if (throwOnCycle)
                    throw new Exception("Cyclic dependency found");
            }
        }
    }
}