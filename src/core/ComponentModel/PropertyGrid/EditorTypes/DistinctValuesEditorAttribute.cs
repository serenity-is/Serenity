namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "DistinctValues" editor
/// and also defines an automatic lookup script for row fields.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class DistinctValuesEditorAttribute : LookupEditorBaseAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "Lookup";

    /// <summary>
    /// Initializes a new instance of the <see cref="DistinctValuesEditorAttribute"/> class.
    /// </summary>
    public DistinctValuesEditorAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DistinctValuesEditorAttribute"/> class.
    /// </summary>
    /// <param name="rowType">Type of the row.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <exception cref="ArgumentNullException">
    /// rowType
    /// or
    /// propertyName
    /// </exception>
    public DistinctValuesEditorAttribute(Type rowType, string propertyName)
        : base(Key)
    {
        RowType = rowType ?? throw new ArgumentNullException("rowType");
        PropertyName = propertyName ?? throw new ArgumentNullException("propertyName");
    }

    /// <summary>
    /// RowType that this editor will get values from
    /// </summary>
    public Type? RowType { get; set; }

    /// <summary>
    /// Property name that this editor will get values from
    /// </summary>
    public string? PropertyName { get; set; }

    /// <summary>
    /// Permission key required to access this lookup script.
    /// Use special value "?" for all logged-in users.
    /// Use special value "*" for anyone including not logged-in users.
    /// </summary>
    public string? Permission { get; set; }

    /// <summary>
    /// Cache duration in seconds
    /// </summary>
    public int Expiration { get; set; }
}