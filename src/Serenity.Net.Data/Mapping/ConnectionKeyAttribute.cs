namespace Serenity.Data;

/// <summary>
/// Determines the connection key used for a class
/// </summary>
/// <seealso cref="Attribute" />
public class ConnectionKeyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionKeyAttribute"/> class.
    /// </summary>
    /// <param name="connectionKey">The connection key.</param>
    public ConnectionKeyAttribute(string connectionKey)
    {
        Value = connectionKey ?? throw new ArgumentNullException(nameof(connectionKey));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionKeyAttribute"/> class
    /// with a type that has a ConnectionKey attribute to read the connection key from.
    /// </summary>
    /// <param name="sourceType">Type of the source.</param>
    /// <exception cref="ArgumentNullException">sourceType is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">sourceType has no ConnectionKey attribute.</exception>
    public ConnectionKeyAttribute(Type sourceType)
    {
        if (sourceType == null)
            throw new ArgumentNullException("sourceType");

        var attr = sourceType.GetCustomAttribute<ConnectionKeyAttribute>(true);
        if (attr == null)
            throw new ArgumentOutOfRangeException("sourceType",
                "ConnectionKeyAttribute is created with source type " + sourceType.Name +
                ", but that class has no ConnectionKey attribute");

        Value = attr.Value;
        SourceType = sourceType;
    }

    /// <summary>
    /// Gets the connection key.
    /// </summary>
    /// <value>
    /// The connection.
    /// </value>
    public string Value { get; private set; }

    /// <summary>
    /// Gets the source type with connection key attribute if any.
    /// </summary>
    /// <value>
    /// The type of the source.
    /// </value>
    public Type SourceType { get; private set; }
}