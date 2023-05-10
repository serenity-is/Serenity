namespace Serenity.ComponentModel;

/// <summary>
/// Sets the column as sorted by default, and its index among sorted columns.
/// Negative values means descending sort. Applies only to columns.
/// </summary>
/// <seealso cref="Attribute" />
public class SortOrderAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortOrderAttribute"/> class.
    /// </summary>
    /// <param name="sortOrder">The sort order.</param>
    public SortOrderAttribute(int sortOrder)
    {
        SortOrder = sortOrder;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortOrderAttribute"/> class.
    /// </summary>
    /// <param name="sortOrder">The sort order.</param>
    /// <param name="descending">if set to <c>true</c> descending sort.</param>
    public SortOrderAttribute(int sortOrder, bool descending)
        : this(descending ? -sortOrder : sortOrder)
    {
    }

    /// <summary>
    /// Gets the sort order.
    /// </summary>
    /// <value>
    /// The sort order.
    /// </value>
    public int SortOrder { get; private set; }

    /// <summary>
    /// Gets a value indicating whether sort is descending.
    /// </summary>
    /// <value>
    ///   <c>true</c> if descending; otherwise, <c>false</c>.
    /// </value>
    public bool Descending => SortOrder < 0;
}
