namespace Serenity;

/// <summary>
/// Attribute to access the default section key for an option class
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class DefaultSectionKeyAttribute : Attribute
{
    /// <summary>
    /// Creates a new instance of the attribute
    /// </summary>
    /// <param name="sectionKey">Section key</param>
    public DefaultSectionKeyAttribute(string sectionKey)
    {
        SectionKey = sectionKey ?? throw new ArgumentNullException(nameof(sectionKey));
    }

    /// <summary>
    /// Gets the default section key
    /// </summary>
    public string SectionKey { get; }
}