namespace Serenity.Data.Mapping;

/// <summary>
/// Marks the property so that it should be included in quick text searches.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="QuickSearchAttribute"/> class.
/// </remarks>
/// <param name="searchType">Type of the search.</param>
/// <param name="numericOnly">The numeric only.</param>
/// <param name="isExplicit">if set to <c>true</c> [is explicit].</param>
public class QuickSearchAttribute(SearchType searchType = SearchType.Auto, int numericOnly = -1, bool isExplicit = false) : Attribute
{

    /// <summary>
    /// Gets the type of the search.
    /// </summary>
    /// <value>
    /// The type of the search.
    /// </value>
    public SearchType SearchType { get; private set; } = searchType;

    /// <summary>
    /// Gets the numeric only.
    /// </summary>
    /// <value>
    /// The numeric only.
    /// </value>
    public bool? NumericOnly { get; private set; } = numericOnly < 0 ? null : (numericOnly > 0);

    /// <summary>
    /// Gets a value indicating whether this instance is explicit.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is explicit; otherwise, <c>false</c>.
    /// </value>
    public bool IsExplicit { get; private set; } = isExplicit;
}