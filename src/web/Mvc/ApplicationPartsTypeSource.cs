
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Serenity.Web;

/// <summary>
/// Implementation of a type source that uses <see cref="ApplicationPartManager"/> to
/// get assemblies. Note that it only includes assemblies that are marked with
/// <see cref="TypeSourceAssemblyAttribute"/>, which is automatically added to
/// assemblies that reference the Serenity.Net.Web NuGet package (or Serenity.Net.Web.targets).
/// </summary>
public class ApplicationPartsTypeSource(ApplicationPartManager partManager,
    bool topologicalSort = true, IFeatureToggles featureToggles = null)
    : BaseAssemblyTypeSource(featureToggles)
{
    /// <summary>
    /// Gets the application part manager.
    /// </summary>
    public readonly ApplicationPartManager PartManager = partManager
        ?? throw new ArgumentNullException(nameof(partManager));

    /// <summary>
    /// Gets all the assemblies from the application part manager.
    /// </summary>
    protected virtual IEnumerable<Assembly> GetApplicationPartAssemblies()
    {
        return PartManager.ApplicationParts
            .OfType<AssemblyPart>()
            .Select(x => x.Assembly);
    }

    /// <summary>
    /// Returns <c>true</c> for assemblies that are marked with <see cref="TypeSourceAssemblyAttribute"/>.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    protected virtual bool IsTypeSourceAssembly(Assembly assembly)
    {
        return assembly.IsDefined(typeof(TypeSourceAssemblyAttribute));
    }

    /// <summary>
    /// Gets the set of implicitly included assemblies, by default from
    /// Serenity.Net.Core to Serenity.Net.Web.
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
        var assemblies = Enumerable.Reverse(GetImplicitAssemblies()
            .Concat(GetApplicationPartAssemblies()
                .Where(IsTypeSourceAssembly)));

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
    /// Sorts assemblies by dependency order.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    protected virtual IEnumerable<Assembly> TopologicalSort(IEnumerable<Assembly> assemblies)
    {
        return Reflection.AssemblySorter.Sort(assemblies);
    }
}