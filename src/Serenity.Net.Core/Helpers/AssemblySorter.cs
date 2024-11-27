namespace Serenity.Reflection;

/// <summary>
/// Sorts assemblies based on their dependencies
/// </summary>
public class AssemblySorter
{
    private class AssemblyItem(Assembly item)
    {
        public readonly Assembly Item = item;
        public readonly IList<AssemblyItem> Dependencies = [];
        public readonly string FullName = item.FullName;
    }

    /// <summary>
    /// Sorts the specified assemblies based on their dependencies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns></returns>
    public static IEnumerable<Assembly> Sort(IEnumerable<Assembly> assemblies)
    {
        var assemblyItems = assemblies.Select(a => new AssemblyItem(a)).ToArray();

        foreach (var item in assemblyItems)
        {
            foreach (var reference in item.Item.GetReferencedAssemblies())
            {
                var dependency = assemblyItems.SingleOrDefault(i => i.FullName == reference.FullName);

                if (dependency != null)
                    item.Dependencies.Add(dependency);
            }
        }

        return TSort(assemblyItems, i => i.Dependencies).Select(x => x.Item);
    }

    /// <summary>
    /// A generic dependency sort function
    /// </summary>
    /// <typeparam name="T">Type of items</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="dependencies">The dependencies.</param>
    /// <returns></returns>
    public static IEnumerable<T> TSort<T>(IEnumerable<T> source,
        Func<T, IEnumerable<T>> dependencies)
    {
        var sorted = new List<T>();
        var visited = new HashSet<T>();

        foreach (var item in source)
            Visit(item, visited, sorted, dependencies);

        return sorted;
    }

    private static void Visit<T>(T item, HashSet<T> visited, List<T> sorted,
        Func<T, IEnumerable<T>> dependencies)
    {
        if (!visited.Add(item))
            return;

        foreach (var dep in dependencies(item))
            Visit(dep, visited, sorted, dependencies);

        sorted.Add(item);
    }
}