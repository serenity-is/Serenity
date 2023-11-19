namespace Serenity.Data;

/// <summary>
/// Determines non-plural name for an entity.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="InstanceNameAttribute"/> class.
/// </remarks>
/// <param name="instanceName">Name of the instance.</param>
public class InstanceNameAttribute(string instanceName) : Attribute
{

    /// <summary>
    /// Gets the name of the instance.
    /// </summary>
    /// <value>
    /// The name of the instance.
    /// </value>
    public string InstanceName { get; private set; } = instanceName;
}