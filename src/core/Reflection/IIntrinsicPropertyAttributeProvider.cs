namespace Serenity.Reflection;

/// <summary>
/// Marks a composite attribute that can provide additional intrinsic attributes for a target property.
/// The intrinsic attributes should be declared on its <see cref="PropertyAttributes"/> property.
/// </summary>
public interface IIntrinsicPropertyAttributeProvider
{
    /// <summary>
    /// Implementing attributes expose additional intrinsic property attributes by applying them
    /// to this property. This is useful because some attributes can only target properties due to
    /// <see cref="AttributeUsageAttribute"/> restrictions.
    /// </summary>
    object PropertyAttributes { get; }
}