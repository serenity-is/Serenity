namespace Serenity.Services;

/// <summary>
/// Specifies how an entity is read when it is used as a source of another query, e.g. by an entity query of a
/// data source, or that it can't be used so. See <see cref="EntityQueryMode"/>.
/// </summary>
/// <param name="mode">The mode.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class EntityQueryAttribute(EntityQueryMode mode) : Attribute
{
    /// <summary>
    /// Gets the mode.
    /// </summary>
    public EntityQueryMode Mode { get; } = mode;
}
