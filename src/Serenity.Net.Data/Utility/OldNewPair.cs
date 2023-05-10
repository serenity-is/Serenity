namespace Serenity.Data;

/// <summary>
/// A pair of old new values
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
public struct OldNewPair<TItem>
{
    private readonly TItem _old;
    private readonly TItem _new;

    /// <summary>
    /// Initializes a new instance of the <see cref="OldNewPair{TItem}"/> struct.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <param name="n">The n.</param>
    public OldNewPair(TItem o, TItem n)
    {
        _old = o;
        _new = n;
    }

    /// <summary>
    /// Gets the old.
    /// </summary>
    /// <value>
    /// The old.
    /// </value>
    public TItem Old => _old;

    /// <summary>
    /// Gets the new.
    /// </summary>
    /// <value>
    /// The new.
    /// </value>
    public TItem New => _new;
}