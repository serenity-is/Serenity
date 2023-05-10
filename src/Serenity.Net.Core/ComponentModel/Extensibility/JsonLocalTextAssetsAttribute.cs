namespace Serenity.ComponentModel;

/// <summary>
/// Registers a folder with json local text files packed as static web assets
/// for the assembly
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class JsonLocalTextAssetsAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonLocalTextAssetsAttribute"/> class.
    /// </summary>
    public JsonLocalTextAssetsAttribute(string path)
    {
        Path = path ?? throw new ArgumentNullException(nameof(path));
    }

    /// <summary>
    /// The path for static web assets folder with json local text files
    /// </summary>
    public string Path { get; }
}