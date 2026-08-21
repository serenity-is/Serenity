using Microsoft.Extensions.DependencyInjection;

namespace Serenity.ComponentModel;

/// <summary>
/// Enables auto registering for the implementation type this attribute is placed on.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public abstract class RegisterServiceAttribute : Attribute
{
    /// <summary>
    /// Creates a new instance of RegisterServiceAttribute.
    /// </summary>
    public RegisterServiceAttribute()
    {
    }

    /// <summary>
    /// Indicates whether to replace existing registration. Note that when ReplaceExisting is true,
    /// SkipExisting is ignored.
    /// </summary>
    public bool ReplaceExisting { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to skip registration if a registration for the service
    /// already exists (default true), e.g. use TryAddSingleton etc. methods instead of AddSingleton.
    /// </summary>
    public bool SkipExisting { get; set; } = true;

    /// <summary>
    /// The list of types that this service will be registered for.
    /// If no types are specified, the system will try to auto determine the interface.
    /// It will skip interfaces from those in "System." / "Microsoft." namespaces.
    /// It will also skip interfaces that implement IRequestHandler, e.g. service request handlers,
    /// as they should be auto-registered by AddServiceHandlers.
    /// If only one interface is found, it will use that, or if multiple is found, it will prefer
    /// the one with I{ClassName}. Otherwise an exception will be raised.
    /// Note that all the types specified must be assignable from the current type.
    /// </summary>
    public Type[]? Types { get; set; }
    
    /// <summary>
    /// Gets the lifetime, which is transient by default.
    /// </summary>
    public ServiceLifetime Lifetime { get; protected set; } = ServiceLifetime.Transient;

    /// <summary>
    /// The key identifier for keyed service registration.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Register the type itself as concrete implementation for itself in addition to
    /// any <see cref="Types" /> specified, or the type's auto-detected interface.
    /// To register only itself with no interfaces, pass an empty array as Types: <c>[]</c>.
    /// Note that AsSelf cannot be used with keyed service registrations and Key property will be ignored.
    /// </summary>
    public bool AsSelf { get; set; }

    /// <summary>
    /// Gets or sets the order of registration. Services with lower order will be registered first.
    /// This can be used to ensure that certain services are registered before others, e.g. when multiple implementations of the same service type are registered and the order matters.
    /// The default value is 0, and a higher value means a later registration.
    /// </summary>
    public int Order { get; set; } = 0;
}