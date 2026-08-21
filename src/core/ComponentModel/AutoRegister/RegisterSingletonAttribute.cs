using Microsoft.Extensions.DependencyInjection;

namespace Serenity.ComponentModel;

/// <summary>
/// Enables auto registering for the implementation type this attribute is placed on by using
/// the {Try}AddSingleton{Keyed} method.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class RegisterSingletonAttribute : RegisterServiceAttribute
{
    /// <summary>
    /// Creates a new instance of the attribute.
    /// </summary>
    public RegisterSingletonAttribute()
    {
        Lifetime = ServiceLifetime.Singleton;
    }

    /// <summary>
    /// Creates a new instance of the attribute for the specified types.
    /// </summary>
    /// <param name="types">Service types.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="types"/> is null.</exception>
    public RegisterSingletonAttribute(params Type[] types) : this()
    {
        Types = types ?? throw new ArgumentNullException(nameof(types));
    }
}