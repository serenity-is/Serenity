namespace Serenity;

/// <summary>
/// Attribute to access the default section key for an option class
/// </summary>
/// <remarks>
/// Creates a new instance of the attribute
/// </remarks>
/// <param name="sectionKey">Section key</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class DefaultSectionKeyAttribute(string sectionKey) : Attribute
{

    /// <summary>
    /// Gets the default section key
    /// </summary>
    public string SectionKey { get; } = sectionKey ?? throw new ArgumentNullException(nameof(sectionKey));
}