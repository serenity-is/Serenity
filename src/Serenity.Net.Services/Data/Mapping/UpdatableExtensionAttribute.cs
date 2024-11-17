namespace Serenity.Data.Mapping;

/// <summary>
/// Marks the JOIN as an updatable extension.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class UpdatableExtensionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatableExtensionAttribute"/> class.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="rowType">Type of the row.</param>
    public UpdatableExtensionAttribute(string alias, Type rowType)
    {
        if (string.IsNullOrEmpty(alias))
            throw new ArgumentNullException(nameof(alias));

        Alias = alias;
        RowType = rowType ?? throw new ArgumentNullException(nameof(rowType));
    }

    /// <summary>
    /// Gets the type of the row.
    /// </summary>
    /// <value>
    /// The type of the row.
    /// </value>
    public Type RowType { get; private set; }

    /// <summary>
    /// The join alias in this row that brings in extension table fields as view fields
    /// </summary>
    public string Alias { get; private set; }

    /// <summary>
    /// Name of the key field in this table. 
    /// If not specified, ID field of this table will be used.
    /// </summary>
    public string ThisKey { get; set; }

    /// <summary>
    /// Name of the key field in extension table. 
    /// If not specified, ID field of extension table is assumed,
    /// unless there is a field with matching name to ThisKey in extension table.
    /// </summary>
    public string OtherKey { get; set; }

    /// <summary>
    /// Name of a field in extension table that will be filtered in 
    /// extension table in addition to key.
    /// For example, if you have a CustomerAddresses table, and
    /// your join condition is T0.CustomerID = ca.CustomerID and
    /// ca.AddressType = 'Billing', your FilterField is AddressType
    /// and your FilterValue is 'Billing'
    /// </summary>
    public string FilterField { get; set; }

    /// <summary>
    /// Constant value of a field in extension table that will be 
    /// filtered in extension table in addition to key.
    /// For example, if you have a CustomerAddresses table, and
    /// your join condition is T0.CustomerID = ca.CustomerID and
    /// ca.AddressType = 'Billing', your FilterField is AddressType
    /// and your FilterValue is 'Billing'
    /// </summary>
    public object FilterValue { get; set; }

    /// <summary>
    /// This extension should only be inserted if this field is equal to PresenceValue
    /// For example, you might have a PersonType column in PersonRow and 
    /// student record should only be created (if not already) if PersonType = "Student".
    /// </summary>
    public string PresenceField { get; set; }

    /// <summary>
    /// This extension should only be inserted if PresenceField value is equal to this one.
    /// For example, you might have a PersonType column in PersonRow and 
    /// student record should only be created (if not already) if PersonType = "Student".
    /// </summary>
    public object PresenceValue { get; set; }

    /// <summary>
    /// Delete extension record if this record is deleted
    /// </summary>
    public bool CascadeDelete { get; set; }
}