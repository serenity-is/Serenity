namespace Serenity.PropertyGrid;

/// <summary>
/// Basic property processor
/// </summary>
public partial class BasicPropertyProcessor : PropertyProcessor
{
    /// <summary>
    /// Sets properties of a PropertyItem by analysing a property source
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="item">The item.</param>
    public override void Process(IPropertySource source, PropertyItem item)
    {
        SetAlignment(source, item);
        SetAllowHide(source, item);
        SetCategory(source, item);
        SetCollapsible(source, item);
        SetTab(source, item);
        SetCssClass(source, item);
        SetDefaultValue(source, item);
        SetEditLink(source, item);
        SetEditing(source, item);
        SetFiltering(source, item);
        SetFormatting(source, item);
        SetFormCssClass(source, item);
        SetGrouping(source, item);
        SetHideOnInsert(source, item);
        SetHideOnUpdate(source, item);
        SetHint(source, item);
        SetInsertable(source, item);
        SetInsertPermission(source, item);
        SetOneWay(source, item);
        SetPlaceholder(source, item);
        SetReadOnly(source, item);
        SetReadPermission(source, item);
        SetRequired(source, item);
        SetResizable(source, item);
        SetSorting(source, item);
        SetSummaryType(source, item);
        SetTitle(source, item);
        SetUpdatable(source, item);
        SetUpdatePermission(source, item);
        SetVisible(source, item);
        SetWidth(source, item);
    }

    /// <summary>
    /// Gets the priority.
    /// </summary>
    /// <value>
    /// The priority.
    /// </value>
    public override int Priority => 10;
}