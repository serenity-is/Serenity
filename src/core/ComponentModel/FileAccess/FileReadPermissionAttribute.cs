namespace Serenity.Data;

/// <summary>
/// A subclass of <see cref="FileReadAccessAttribute"/> that specifies a permission
/// </summary>
public class FileReadPermissionAttribute : FileReadAccessAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileReadPermissionAttribute"/> class.
    /// </summary>
    public FileReadPermissionAttribute(string permission) : base()
    {
        Permission = permission ?? throw new ArgumentNullException(nameof(permission));
    }
}