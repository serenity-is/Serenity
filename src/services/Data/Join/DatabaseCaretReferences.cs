using System.Diagnostics.CodeAnalysis;

namespace Serenity.Data;

/// <summary>
/// Helper class for replacing database caret references in format [^ConnectionKey] in SQL expressions.
/// </summary>
public class DatabaseCaretReferences
{
    /// <summary>
    /// Replaces caret references like [^ConnectionKey] in the specified expression with actual database names.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>Replaced expression.</returns>
    [return:NotNullIfNotNull(nameof(expression))]
    public static string? Replace(string? expression)
    {
        if (expression == null || expression.IndexOf('^') < 0)
            return expression;

        return BracketLocator.ReplaceBracketContents(expression, '^', contents =>
        {
            var idx = contents.IndexOf('^');
            if (idx < 0)
                return contents;

            string? connectionKey = null;

            if (idx != 0)
                connectionKey = contents[..idx];

            string? databaseName;

            if (!string.IsNullOrEmpty(connectionKey))
            {
                databaseName = GetDatabaseName?.Invoke(connectionKey);
                if (!string.IsNullOrEmpty(databaseName))
                    return databaseName;
            }

            if (idx < contents.Length - 1)
                return contents[(idx + 1)..];

            return contents;
        });
    }

    /// <summary>
    /// Temporary workaround as this class has no reference to SQL connection strings.
    /// Getter returns the local resolver if any is set through
    /// <see cref="SetLocalGetDatabaseName"/>, otherwise the default one.
    /// The default resolver should only be set on application start.
    /// The local resolver should be used for unit tests.
    /// </summary>
    public static Func<string, string>? GetDatabaseName
    {
        get => localGetDatabaseName.Value ?? defaultGetDatabaseName;
        set => defaultGetDatabaseName = value;
    }

    /// <summary>
    /// Sets the local database name resolver for the current thread and async context.
    /// Useful for background tasks, async methods, and testing to
    /// set the resolver locally and for auto spawned threads.
    /// </summary>
    /// <param name="resolver">The resolver. Can be null.</param>
    /// <returns>The old local resolver, if any.</returns>
    public static Func<string, string>? SetLocalGetDatabaseName(Func<string, string>? resolver)
    {
        var old = localGetDatabaseName.Value;
        localGetDatabaseName.Value = resolver;
        return old;
    }

    /// <summary>The default database name resolver.</summary>
    private static Func<string, string>? defaultGetDatabaseName;

    /// <summary>The local database name resolver for the current thread and async context.</summary>
    private static readonly AsyncLocal<Func<string, string>?> localGetDatabaseName = new();
}