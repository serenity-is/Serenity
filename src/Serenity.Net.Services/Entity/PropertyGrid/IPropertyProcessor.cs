namespace Serenity.PropertyGrid;

/// <summary>
/// Interface for property processors, which sets properties of a PropertyItem
/// object by analysing a IPropertySource object.
/// </summary>
public interface IPropertyProcessor
{
    /// <summary>
    /// Initializes this instance.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Sets properties of a PropertyItem by analysing a property source
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="item">The item.</param>
    void Process(IPropertySource source, PropertyItem item);

    /// <summary>
    /// Called after process in a secondary pass.
    /// </summary>
    void PostProcess();

    /// <summary>
    /// Gets or sets the items.
    /// </summary>
    /// <value>
    /// The items.
    /// </value>
    List<PropertyItem> Items { get; set; }

    /// <summary>
    /// Gets or sets the type property processor is working on.
    /// </summary>
    /// <value>
    /// The type.
    /// </value>
    Type Type { get; set; }

    /// <summary>
    /// Gets or sets the based on row.
    /// </summary>
    /// <value>
    /// The based on row.
    /// </value>
    IRow BasedOnRow { get; set; }

    /// <summary>
    /// Gets the priority. The processors are called based
    /// on this priority.
    /// </summary>
    /// <value>
    /// The priority.
    /// </value>
    int Priority { get; }
}