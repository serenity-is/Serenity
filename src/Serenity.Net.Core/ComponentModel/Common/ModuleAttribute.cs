namespace Serenity.ComponentModel;

/// <summary>
/// Sets the module name for the row. Module name is usually the folder name
/// under ~/Modules folder entity resides in.
/// </summary>
/// <seealso cref="Attribute" />
public class ModuleAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleAttribute"/> class.
    /// </summary>
    /// <param name="module">The module.</param>
    public ModuleAttribute(string module)
    {
        Value = module;
    }

    /// <summary>
    /// Gets the module.
    /// </summary>
    /// <value>
    /// The module.
    /// </value>
    public string Value { get; private set; }
}