namespace Serenity.Data.Mapping;

/// <summary>
/// Specifies a linking set relation (1-N relation of ID values, like a check list.)
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Declares a linking set relation
/// </remarks>
/// <param name="rowType">Linking row type</param>
/// <param name="thisKey">Name of the field in linking row that corresponds to ID in this table</param>
/// <param name="itemKey">Name of the field in linking row that will hold item values in list</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class LinkingSetRelationAttribute(Type rowType, string thisKey, string itemKey) : Attribute
{

    /// <summary>
    /// Gets the type of the row.
    /// </summary>
    /// <value>
    /// The type of the row.
    /// </value>
    public Type RowType { get; private set; } = rowType ?? throw new ArgumentNullException(nameof(rowType));

    /// <summary>
    /// Gets the this key, e.g. name of field in linking set table that this tables ID field corresponds to.
    /// </summary>
    /// <value>
    /// The this key.
    /// </value>
    public string ThisKey { get; private set; } = thisKey ?? throw new ArgumentNullException(nameof(thisKey));

    /// <summary>
    /// Gets the item key, e.g. name of the field in linking set table that contains ID's of selected items.
    /// </summary>
    /// <value>
    /// The item key.
    /// </value>
    public string ItemKey { get; private set; } = itemKey ?? throw new ArgumentNullException(nameof(itemKey));

    /// <summary>
    /// Gets or sets the filter field.
    /// </summary>
    /// <value>
    /// The filter field.
    /// </value>
    public string FilterField { get; set; }

    /// <summary>
    /// Gets or sets the filter value.
    /// </summary>
    /// <value>
    /// The filter value.
    /// </value>
    public object FilterValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to preserve existing order of records on save.
    /// </summary>
    /// <value>
    ///   <c>true</c> if should preserve order; otherwise, <c>false</c>.
    /// </value>
    public bool PreserveOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to handle equality filter.
    /// </summary>
    /// <value>
    ///   <c>true</c> if should handle equality filter; otherwise, <c>false</c>.
    /// </value>
    public bool HandleEqualityFilter { get; set; } = true;

    /// <summary>
    /// Forces deletion of linking row records even if master record uses soft delete.
    /// If false (default) this doesn't delete linking records, as master record might be undeleted.
    /// </summary>
    public bool ForceCascadeDelete { get; set; }
}