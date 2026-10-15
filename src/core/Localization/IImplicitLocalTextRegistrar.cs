
namespace Serenity.Abstractions;

/// <summary>
/// An interface implemented by type attributes that register implicit local text keys,
/// and their default translations, generally during application startup.
/// </summary>
public interface IImplicitLocalTextRegistrar
{
    /// <summary>
    /// Adds local text keys and their default translations to the specified registry.
    /// This is usually called before other registration types like NestedLocalText so 
    /// texts added here might be overridden by subsequent registrations. It is a good
    /// practice to skip adding keys that already exist in the registry.
    /// </summary>
    /// <param name="registry">The local text registry to which the local text keys will be added.</param>
    /// <param name="memberInfo">The type or member info for which to add implicit local text keys.</param>
    void AddLocalTexts(ILocalTextRegistry registry, MemberInfo memberInfo);
}