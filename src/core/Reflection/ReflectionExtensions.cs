namespace Serenity;

/// <summary>
/// Reflection extensions
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    /// Gets the attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <param name="member">The member.</param>
    /// <param name="inherit">if set to <c>true</c> [inherit].</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static TAttribute? GetAttribute<TAttribute>(this MemberInfo member, bool inherit = false) where TAttribute : Attribute
    {
        var attrs = member.GetCustomAttributes(typeof(TAttribute), inherit);
        if (!attrs.Any())
            return null;

        if (attrs.Count() > 1)
            throw new InvalidOperationException(string.Format("{0} has more than 1 of {1} attribute", member.Name, typeof(TAttribute).Name));

        return (TAttribute)attrs.First();
    }
}
