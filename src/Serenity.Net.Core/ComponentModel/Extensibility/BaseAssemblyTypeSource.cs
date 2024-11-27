namespace Serenity.Abstractions;

/// <summary>
/// Base type source implementation that accepts a list of assemblies
/// </summary>
/// <param name="featureToggles">Feature toggles service used to filter types</param>
/// <remarks>
/// Creates a new instance
/// </remarks>
public abstract class BaseAssemblyTypeSource(IFeatureToggles? featureToggles = null) : ITypeSource, IGetAssemblies
{
    /// <summary>
    /// Gets all attributes for assemblies with given type
    /// </summary>
    /// <returns>List of attributes for assemblies</returns>
    public virtual IEnumerable<Attribute> GetAssemblyAttributes(Type attributeType)
    {
        return GetAssemblies().Where(Include).SelectMany(x => x.GetCustomAttributes(attributeType));
    }

    /// <summary>
    /// Used to filter assemblies based on feature toggles
    /// </summary>
    /// <param name="assembly">Assembly to filter</param>
    protected virtual bool Include(Assembly assembly)
    {
        return featureToggles == null ||
            assembly.GetCustomAttribute<RequiresFeatureAttribute>() is not { } attr ||
            featureToggles.IsEnabled(attr.Features, attr.RequireAny);
    }

    /// <summary>
    /// Used to filter types based on feature toggles
    /// </summary>
    /// <param name="type">Type to filter</param>
    protected virtual bool Include(Type type)
    {
        return featureToggles == null ||
            type.GetAttribute<RequiresFeatureAttribute>() is not { } attr ||
            featureToggles.IsEnabled(attr.Features, attr.RequireAny);
    }

    /// <summary>
    /// Gets all types
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<Type> GetTypes()
    {
        return GetAssemblies().Where(Include).SelectMany(x => x.GetTypes().Where(Include));
    }

    /// <summary>
    /// Gets all types that implement an interface
    /// </summary>
    /// <param name="interfaceType">Interface type</param>
    /// <returns>Types with that interface type</returns>
    public virtual IEnumerable<Type> GetTypesWithInterface(Type interfaceType)
    {
        return GetTypes().Where(interfaceType.IsAssignableFrom);
    }

    /// <summary>
    /// Gets all types that has an attribute
    /// </summary>
    /// <param name="attributeType">Attribute type</param>
    /// <returns>Types with that attribute type</returns>
    public virtual IEnumerable<Type> GetTypesWithAttribute(Type attributeType)
    {
        return GetTypes().Where(type => type.GetCustomAttribute(attributeType) != null);
    }

    /// <inheritdoc/>
    public abstract IEnumerable<Assembly> GetAssemblies();
}