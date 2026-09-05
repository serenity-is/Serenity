namespace Serenity.Services;

/// <summary>
/// Specifies the companion (other mode) handler interface for a request processor
/// interface, e.g. <see cref="ISaveRequestProcessorAsync"/> as companion of
/// <see cref="ISaveRequestProcessor"/>, together with a wrapper type that adapts a
/// custom handler of the companion interface to this interface.
/// </summary>
/// <remarks>
/// This attribute is used by <see cref="DefaultHandlerFactory"/> to discover custom
/// request handlers of the other (sync / async) mode, when no custom handler exists
/// for the requested mode. For example when an async behavior requests
/// <see cref="ISaveRequestProcessor"/> for a row, but only an async custom handler
/// implementing <see cref="ISaveRequestProcessorAsync"/> is available, the factory
/// returns the async custom handler wrapped in the specified wrapper type so that
/// custom handler logic is not silently skipped.
/// </remarks>
[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
public class CompanionHandlerTypeAttribute(Type companionType, Type wrapperType) : Attribute
{
    /// <summary>
    /// Gets the companion (other mode) handler interface type.
    /// </summary>
    public Type CompanionType { get; } = companionType ??
        throw new ArgumentNullException(nameof(companionType));

    /// <summary>
    /// Gets the open generic wrapper type that adapts a handler implementing
    /// <see cref="CompanionType"/> to the interface this attribute is applied to.
    /// </summary>
    public Type WrapperType { get; } = wrapperType ??
        throw new ArgumentNullException(nameof(wrapperType));
}
