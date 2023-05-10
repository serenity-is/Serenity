namespace Serenity.Services;

/// <summary>
/// An interface to query list of implicit behaviors registered through the dependency resolver.
/// Implicit behaviors are automatically activated behaviors by querying via their ActivateFor 
/// method. See <see cref="IImplicitBehavior"/>
/// </summary>
public interface IImplicitBehaviorRegistry
{
    /// <summary>
    /// Gets type list of implict behavior (<see cref="IImplicitBehavior"/>) types 
    /// </summary>
    IEnumerable<Type> GetTypes();
}