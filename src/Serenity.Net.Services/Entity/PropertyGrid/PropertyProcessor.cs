namespace Serenity.PropertyGrid;

/// <summary>
/// Base class for property processors, which sets properties of a PropertyItem
/// object by analysing a IPropertySource object.
/// </summary>
/// <seealso cref="IPropertyProcessor" />
public abstract class PropertyProcessor : IPropertyProcessor
{
    /// <inheritdoc/>
    public virtual void Process(IPropertySource source, PropertyItem item)
    {
    }

    /// <inheritdoc/>
    public virtual void Initialize()
    {
    }

    /// <inheritdoc/>
    public virtual void PostProcess()
    {
    }

    /// <inheritdoc/>
    public virtual int Priority => 50;

    /// <inheritdoc/>
    public Type Type { get; set; }

    /// <inheritdoc/>
    public IRow BasedOnRow { get; set; }

    /// <inheritdoc/>
    public List<PropertyItem> Items { get; set; }
}