using System.ComponentModel.DataAnnotations;

namespace Serenity;

/// <summary>
/// Gets what a validation rule may need from the context it is checked in, e.g. the localizer of its message, which the
/// save handlers and the other places that check the rules give as the services of the context.
/// </summary>
public static class ValidationContextExtensions
{
    /// <summary>
    /// Gets the localizer of the messages, or null when the context has none.
    /// </summary>
    /// <param name="context">The context.</param>
    public static ITextLocalizer? GetLocalizer(this ValidationContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        return context.GetService(typeof(ITextLocalizer)) as ITextLocalizer;
    }

    /// <summary>
    /// Gets the connection the value is saved with, e.g. in the transaction of a save handler, or null when the context has none.
    /// </summary>
    /// <param name="context">The context.</param>
    public static IDbConnection? GetConnection(this ValidationContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        return context.GetService(typeof(IDbConnection)) as IDbConnection;
    }

    /// <summary>
    /// Gets the value of another property of the object being validated, e.g. a row, for a rule on more than one field.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The value, or null when the object has no such property.</returns>
    public static object? GetPropertyValue(this ValidationContext context, string propertyName)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        if (string.IsNullOrEmpty(propertyName))
            throw new ArgumentNullException(nameof(propertyName));

        var property = context.ObjectInstance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        return property?.GetIndexParameters().Length == 0 ? property.GetValue(context.ObjectInstance) : null;
    }
}
