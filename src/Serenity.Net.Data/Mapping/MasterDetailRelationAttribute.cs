namespace Serenity.Data.Mapping;

/// <summary>
/// Defines a master detail relation (1-N) between this row and another
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class MasterDetailRelationAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MasterDetailRelationAttribute"/> class.
    /// </summary>
    /// <param name="foreignKey">The property name of the FK field in the detail 
    /// table which matches the PK (id) of the master table. For example, if the two 
    /// tables are 'Order(Id, Date, ...)' and 'OrderDetail(DetailId, OrderId, Qty, ...)' 
    /// it should be 'OrderId'.</param>
    public MasterDetailRelationAttribute(string foreignKey)
    {
        ForeignKey = foreignKey ?? throw new ArgumentNullException(nameof(foreignKey));
        CheckChangesOnUpdate = true;
        ColumnSelection = ColumnSelection.List;
    }

    /// <summary>
    /// Gets the foreign key.
    /// </summary>
    /// <value>
    /// The foreign key.
    /// </value>
    public string ForeignKey { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether to check changes on update.
    /// Disable this if you are getting comparison errors.
    /// </summary>
    /// <value>
    ///   <c>true</c> if to check changes on update; otherwise, <c>false</c>.
    /// </value>
    public bool CheckChangesOnUpdate { get; set; }

    /// <summary>
    /// Gets or sets the column selection.
    /// </summary>
    /// <value>
    /// The column selection.
    /// </value>
    public ColumnSelection ColumnSelection { get; set; }

    /// <summary>
    /// Gets or sets the columns type with properties to include in column selection
    /// </summary>
    /// <value>
    /// The include columns type with properties to include.
    /// </value>
    public Type ColumnsType { get; set; }

    /// <summary>
    /// Gets or sets the comma separated list of include columns.
    /// Obsolete, please prefer IncludeColumnNames or ColumnsType
    /// </summary>
    /// <value>
    /// The include columns.
    /// </value>
    public string IncludeColumns { get; set; }

    /// <summary>
    /// Gets or sets the list of include column names as array.
    /// </summary>
    /// <value>
    /// The include columns.
    /// </value>
    public string[] IncludeColumnNames { get; set; }

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
    /// Optional: override the default behaviour and use a different master id (PK) field 
    /// (i.e. from a unique constraint)
    /// </summary>
    public string MasterKeyField { get; set; }

    /// <summary>
    /// Forces deletion of linking row records even if master record uses soft delete.
    /// If false (default) this doesn't delete linking records, as master record might be undeleted.
    /// </summary>
    public bool ForceCascadeDelete { get; set; }
}
