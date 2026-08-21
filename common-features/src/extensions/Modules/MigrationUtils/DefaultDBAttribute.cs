namespace Serenity.Extensions;

/// <summary>
/// Marks a migration to run on the default database.
/// </summary>
public class DefaultDBAttribute : TargetDBAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDBAttribute"/> class.
    /// </summary>
    public DefaultDBAttribute()
        : base("Default")
    {
    }
}