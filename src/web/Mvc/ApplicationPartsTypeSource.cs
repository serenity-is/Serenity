
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Serenity.Web;

/// <summary>
/// Implementation for a type source that uses ApplicationPartManager to 
/// get assemblies. Note that it only includes assemblies that are marked
/// with TypeSourceAssemblyAttribute which is automatically added to 
/// assemblies that reference Serenity.Net.Web NuGet package (or Serenity.Net.Web.targets).
/// </summary>
public class ApplicationPartsTypeSource(ApplicationPartManager partManager,
    bool topologicalSort = true, IFeatureToggles featureToggles = null)
    : BaseAssemblyTypeSource(featureToggles)
{
    /// <summary>
    /// Gets the application part manager
    /// </summary>
    public readonly ApplicationPartManager PartManager = partManager
        ?? throw new ArgumentNullException(nameof(partManager));

    /// <summary>
    /// Gets all the assemblies from application part manager.
    /// </summary>
    protected virtual IEnumerable<Assembly> GetApplicationPartAssemblies()
    {
        return PartManager.ApplicationParts
            .OfType<AssemblyPart>()
            .Select(x => x.Assembly);
    }

    /// <summary>
    /// Returns true for assemblies that are marked with TypeSourceAssemblyAttribute
    /// </summary>
    /// <param name="assembly">Assembly</param>
    protected virtual bool IsTypeSourceAssembly(Assembly assembly)
    {
        return assembly.IsDefined(typeof(TypeSourceAssemblyAttribute));
    }

    /// <summary>
    /// Gets set of implicitly included assemblies, by default Serenity.Net.Core 
    /// to Serenity.Net.Web
    /// </summary>
    protected virtual IEnumerable<Assembly> GetImplicitAssemblies()
    {
        return WebTypeSource.SerenityNetWebAssemblyChain;
    }

    private class SortCache
    {
        public Assembly[] Unsorted;
        public Assembly[] Sorted;
    }

    private SortCache sortCache;

    /// <inheritdoc />
    public override IEnumerable<Assembly> GetAssemblies()
    {
        var assemblies = GetImplicitAssemblies()
            .Concat(GetApplicationPartAssemblies()
                .Where(IsTypeSourceAssembly)
                .Reverse());

        if (!topologicalSort)
            return assemblies;

        var sortCache = this.sortCache;
        if (sortCache != null &&
            sortCache.Unsorted.SequenceEqual(assemblies))
            return sortCache.Sorted;

        this.sortCache = sortCache = new()
        {
            Unsorted = assemblies.ToArray(),
            Sorted = TopologicalSort(assemblies).ToArray()
        };

        return sortCache.Sorted;
    }

    /// <summary>
    /// Sorts assemblies by dependency order
    /// </summary>
    /// <param name="assemblies">Assemblies</param>
    protected virtual IEnumerable<Assembly> TopologicalSort(IEnumerable<Assembly> assemblies)
    {
        return Reflection.AssemblySorter.Sort(assemblies);
    }
}