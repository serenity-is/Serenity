namespace Serenity;

/// <summary>
/// Reflection extensions.
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    /// Gets the attribute. Obsolete, use GetCustomAttribute(bool)"/> instead.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <param name="member">The member.</param>
    /// <param name="inherit">if set to <c>true</c>, searches the inheritance chain.</param>
    [Obsolete]
    public static TAttribute? GetAttribute<TAttribute>(this MemberInfo member, bool inherit = false) where TAttribute : Attribute
    {
        return member.GetCustomAttribute<TAttribute>(inherit);
    }
}
