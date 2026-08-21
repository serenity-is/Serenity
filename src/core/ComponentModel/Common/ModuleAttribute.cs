namespace Serenity.ComponentModel;

/// <summary>
/// Sets the module name for the row. The module name is usually the folder name
/// under the ~/Modules folder that the entity resides in.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="ModuleAttribute"/> class.
/// </remarks>
/// <param name="module">The module.</param>
public class ModuleAttribute(string module) : Attribute
{

    /// <summary>
    /// Gets the module.
    /// </summary>
    /// <value>
    /// The module.
    /// </value>
    public string Value { get; private set; } = module;
}