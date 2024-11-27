namespace Serenity.Data;

/// <summary>
/// This attribute marks a row so that when it is Inserted/Updated/Deleted
/// through handler, its related cache, if any should be cleared. 
/// It doesn't turn on/off caching. A sample of related cached item to a row, might
/// be its lookup if any.
/// </summary>
public class TwoLevelCachedAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TwoLevelCachedAttribute"/> class.
    /// </summary>
    public TwoLevelCachedAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TwoLevelCachedAttribute"/> class.
    /// </summary>
    /// <param name="generationKeys">The generation keys.</param>
    public TwoLevelCachedAttribute(params string[] generationKeys)
    {
        GenerationKeys = generationKeys;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TwoLevelCachedAttribute"/> class.
    /// </summary>
    /// <param name="linkedRows">Types of linked rows</param>
    public TwoLevelCachedAttribute(params Type[] linkedRows)
    {
        LinkedRows = linkedRows;
    }

    /// <summary>
    /// Gets the generation keys.
    /// </summary>
    /// <value>
    /// The generation keys.
    /// </value>
    public string[] GenerationKeys { get; set; }

    /// <summary>
    /// Gets the types of the linked rows.
    /// </summary>
    /// <value>
    /// The linked row types.
    /// </value>
    public Type[] LinkedRows { get; set; }
}
