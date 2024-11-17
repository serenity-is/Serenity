namespace Serenity.Data;

/// <summary>
/// Parameter struct
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Parameter"/> struct.
/// </remarks>
/// <param name="name">The name.</param>
public readonly struct Parameter(string name)
{
    private readonly string name = name;

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public readonly string Name => name;
}
