namespace Serenity.Data;

/// <summary>
/// Parameter struct
/// </summary>
public struct Parameter
{
    private readonly string name;

    /// <summary>
    /// Initializes a new instance of the <see cref="Parameter"/> struct.
    /// </summary>
    /// <param name="name">The name.</param>
    public Parameter(string name)
    {
        this.name = name;
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name => name;
}
