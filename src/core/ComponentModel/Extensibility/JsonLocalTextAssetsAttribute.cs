namespace Serenity.ComponentModel;

/// <summary>
/// Registers a folder with JSON local text files packed as static web assets
/// for the assembly.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="JsonLocalTextAssetsAttribute"/> class.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class JsonLocalTextAssetsAttribute(string path) : Attribute
{

    /// <summary>
    /// The path for the static web assets folder with JSON local text files.
    /// </summary>
    public string Path { get; } = path ?? throw new ArgumentNullException(nameof(path));
}