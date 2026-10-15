namespace Serenity.Localization;

/// <summary>
/// Contains initialization methods for adding implicit local text keys defined by attributes implementing <see cref="IImplicitLocalTextRegistrar"/>.
/// </summary>
public static class ImplicitLocalTextRegistration
{
    /// <summary>
    /// Adds implicit local text keys defined by attributes implementing <see cref="IImplicitLocalTextRegistrar"/> to the specified registry.
    /// This method iterates through all types provided by the <paramref name="typeSource"/> and invokes the <see cref="IImplicitLocalTextRegistrar.AddLocalTexts"/> method
    /// on each attribute implementing <see cref="IImplicitLocalTextRegistrar"/>.
    /// </summary>
    /// <param name="registry">The local text registry to which the local text keys will be added.</param>
    /// <param name="typeSource">The type source to search for types with implicit local text registrars.</param>
    /// <exception cref="ArgumentNullException">Thrown if either <paramref name="registry"/> or <paramref name="typeSource"/> is null.</exception>
    public static void AddImplicitTexts(this ILocalTextRegistry registry, ITypeSource typeSource)
    {
        if (registry is null)
            throw new ArgumentNullException(nameof(registry));
        if (typeSource is null)
            throw new ArgumentNullException(nameof(typeSource));

        foreach (var type in typeSource.GetTypes())
        {
            foreach (var attr in type.GetCustomAttributes(typeof(IImplicitLocalTextRegistrar), inherit: false))
            {
                if (attr is IImplicitLocalTextRegistrar registrar)
                    registrar.AddLocalTexts(registry, type);
            }
        }
    }
}