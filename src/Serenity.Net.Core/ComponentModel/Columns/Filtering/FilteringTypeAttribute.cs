namespace Serenity.ComponentModel;

/// <summary>
/// Sets filtering type for the field
/// </summary>
/// <seealso cref="Attribute" />
public class FilteringTypeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilteringTypeAttribute"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    public FilteringTypeAttribute(string type)
    {
        FilteringType = type;
    }

    /// <summary>
    /// Sets the parameters. This is called by filtering system to pass
    /// additional parameters / options to the filtering object
    /// </summary>
    /// <param name="formatterParams">The formatter parameters.</param>
    public virtual void SetParams(IDictionary<string, object?> formatterParams)
    {
    }

    /// <summary>
    /// Gets the type of the filtering.
    /// </summary>
    /// <value>
    /// The type of the filtering.
    /// </value>
    public string FilteringType { get; private set; }
}
