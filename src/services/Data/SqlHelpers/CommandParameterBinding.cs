using System.Collections.Concurrent;

namespace Serenity.Data;

/// <summary>
/// Makes commands bind parameters by their names. Oracle's provider binds them by their positions by default, so an
/// expression that repeats a parameter or uses parameters in a different order than they were added, like a DATEDIFF
/// that uses its end date before its start date, would get the wrong values. The property is set by reflection to
/// not reference the provider.
/// </summary>
internal static class CommandParameterBinding
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo?> bindByNameProperty = new();

    /// <summary>
    /// Sets the BindByName property of the command if it has one.
    /// </summary>
    /// <param name="command">The command.</param>
    public static void BindByName(IDbCommand command)
    {
        var property = bindByNameProperty.GetOrAdd(command.GetType(), type =>
            type.GetProperty("BindByName", BindingFlags.Public | BindingFlags.Instance) is { CanWrite: true } p &&
                p.PropertyType == typeof(bool) ? p : null);

        property?.SetValue(command, true);
    }
}
