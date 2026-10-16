
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace Serenity.Web;

/// <summary>
/// Implementation of a type source that uses <see cref="ApplicationPartManager"/> to
/// get assemblies. Note that it only includes assemblies that are marked with
/// <see cref="TypeSourceAssemblyAttribute"/>, which is automatically added to
/// assemblies that reference the Serenity.Net.Web NuGet package (or Serenity.Net.Web.targets).
/// </summary>
public class ApplicationPartsTypeSource(ApplicationPartManager partManager,
    bool topologicalSort = true, IFeatureToggles? featureToggles = null, bool tryPartRecovery = true)
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

    // Note: the Razor SDK's FindAssembliesWithReferencesTo / ReferenceResolver resolves
    // assemblies that reference MVC either directly OR transitively (excluding transitive
    // references through framework references). We only check direct references here, so an
    // assembly whose only MVC reference is transitive through a non-framework assembly (e.g.
    // one that references Serenity.Net.Web but no MVC assembly directly) is not recovered.
    // This is currently not handled, as feature assemblies normally use MVC types directly.
    private static bool ReferencesMvc(Assembly assembly)
    {
        return assembly.GetReferencedAssemblies().Any(x =>
            x.Name is not null && x.Name.StartsWith("Microsoft.AspNetCore.Mvc", StringComparison.Ordinal));
    }

    private static bool IsExcludedAssembly(Assembly assembly)
    {
        return IsExcludedName(assembly.GetName().Name);
    }

    private static bool IsExcludedName(string? name)
    {
        // framework assemblies should never be registered as application parts, even though
        // they may be copied next to the application in self contained deployments
        return name is not null &&
            (name.StartsWith("System.", StringComparison.Ordinal) ||
             name.StartsWith("Microsoft.AspNetCore.", StringComparison.Ordinal));
    }

    private static bool FileReferencesMvc(string path)
    {
        // read the assembly references from metadata without loading the assembly, so that
        // we don't pull in every dependency just to check if it references MVC
        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete);
            using var peReader = new PEReader(stream);
            if (!peReader.HasMetadata)
                return false;

            var metadata = peReader.GetMetadataReader();
            foreach (var handle in metadata.AssemblyReferences)
            {
                var reference = metadata.GetAssemblyReference(handle);
                if (metadata.GetString(reference.Name)
                    .StartsWith("Microsoft.AspNetCore.Mvc", StringComparison.Ordinal))
                    return true;
            }
        }
        catch (Exception)
        {
            // not a managed assembly or unreadable
        }

        return false;
    }

    private static HashSet<string> GetDepsAssemblyFileNames()
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var depsFiles = AppContext.GetData("APP_CONTEXT_DEPS_FILES") as string;
        if (string.IsNullOrEmpty(depsFiles))
            return names;

        // the first deps file belongs to the application itself
        var appDeps = depsFiles.Split(';')[0];
        if (string.IsNullOrEmpty(appDeps) || !File.Exists(appDeps))
            return names;

        try
        {
            using var document = JsonDocument.Parse(File.ReadAllBytes(appDeps));
            CollectAssemblyNames(document.RootElement, names);
        }
        catch (JsonException)
        {
        }

        return names;
    }

    private static void CollectAssemblyNames(JsonElement element, HashSet<string> names)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    // assembly assets are the property names ending with .dll under targets
                    if (property.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                        names.Add(Path.GetFileName(property.Name));
                    CollectAssemblyNames(property.Value, names);
                }
                break;
            case JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                    CollectAssemblyNames(item, names);
                break;
        }
    }

    private static readonly object ensureLock = new();
    private volatile bool applicationPartsEnsured;

    /// <summary>
    /// Tries to recover application parts when the generated application parts assembly info
    /// (e.g. <c>*.MvcApplicationPartsAssemblyInfo.cs</c>) was not included in the build.
    /// In that case the <see cref="ApplicationPartManager"/> only contains the entry assembly,
    /// which makes pages and navigation items from referenced assemblies disappear. This reads
    /// the application deps file, finds the assemblies that reference MVC and adds them to the
    /// part manager, just like the Razor SDK would have done at build time.
    /// It is only attempted once, and only when the entry assembly has no
    /// <see cref="ApplicationPartAttribute"/> at all and this assembly (Serenity.Net.Web) is
    /// missing from the part manager. Concurrent callers block until the recovery completes.
    /// </summary>
    protected virtual void EnsureApplicationParts()
    {
        if (!tryPartRecovery || applicationPartsEnsured)
            return;

        lock (ensureLock)
        {
            if (applicationPartsEnsured)
                return;

            var assemblyParts = PartManager.ApplicationParts.OfType<AssemblyPart>().ToList();
            if (assemblyParts.Count == 0)
                return;

            try
            {
                var main = assemblyParts[0].Assembly;

                // The Razor SDK stamps the entry assembly with ApplicationPartAttribute(s) for each
                // referenced assembly. If they are present, discovery worked and there is nothing to do.
                if (main.GetCustomAttributes<ApplicationPartAttribute>().Any())
                    return;

                // If this assembly (Serenity.Net.Web) is already a part, discovery seems to have worked.
                var self = typeof(ApplicationPartsTypeSource).Assembly;
                if (assemblyParts.Any(x => x.Assembly == self))
                    return;

                // The part manager only contains the entry assembly, so application parts discovery
                // failed at build time. Read the application deps file to find the assemblies that
                // were part of the build and add the ones referencing MVC as application parts,
                // just like the Razor SDK would have done. We can't rely on the reference closure
                // as the compiler elides references to project assemblies that are not used
                // directly from the entry assembly.
                var loaded = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => !string.IsNullOrEmpty(x.Location))
                    .GroupBy(x => Path.GetFileName(x.Location), StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key,
                        g => g.FirstOrDefault(x => x.Location!.StartsWith(AppContext.BaseDirectory,
                            StringComparison.OrdinalIgnoreCase)) ?? g.First(),
                        StringComparer.OrdinalIgnoreCase);

                foreach (var name in GetDepsAssemblyFileNames())
                {
                    if (IsExcludedName(Path.GetFileNameWithoutExtension(name)))
                        continue;

                    if (!loaded.TryGetValue(name, out var assembly))
                    {
                        var path = Path.Combine(AppContext.BaseDirectory, name);
                        if (!File.Exists(path))
                            continue;

                        // check the references from metadata first, so that we don't load
                        // assemblies that are not application parts, along with their dependencies
                        if (!FileReferencesMvc(path))
                            continue;

                        try
                        {
                            assembly = Assembly.LoadFrom(path);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    if (assembly is null ||
                        IsExcludedAssembly(assembly) ||
                        !ReferencesMvc(assembly) ||
                        PartManager.ApplicationParts.OfType<AssemblyPart>().Any(x => x.Assembly == assembly))
                        continue;

                    try
                    {
                        foreach (var part in ApplicationPartFactory.GetApplicationPartFactory(assembly)
                            .GetApplicationParts(assembly))
                            PartManager.ApplicationParts.Add(part);
                    }
                    catch
                    {
                        // ignore assemblies that do not provide valid application parts
                    }
                }
            }
            catch
            {
                // recovery is best effort, never break startup because of it
            }
            finally
            {
                applicationPartsEnsured = true;
            }
        }
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
        public required Assembly[] Unsorted;
        public required Assembly[] Sorted;
    }

    private SortCache? sortCache;

    /// <inheritdoc />
    public override IEnumerable<Assembly> GetAssemblies()
    {
        EnsureApplicationParts();

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
            Unsorted = [.. assemblies],
            Sorted = [.. TopologicalSort(assemblies)]
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