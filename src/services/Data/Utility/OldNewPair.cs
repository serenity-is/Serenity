namespace Serenity.Data;

/// <summary>
/// A pair of old and new values.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="OldNewPair{TItem}"/> struct.
/// </remarks>
/// <param name="o">The old value.</param>
/// <param name="n">The new value.</param>
public readonly struct OldNewPair<TItem>(TItem o, TItem n)
{
    private readonly TItem _old = o;
    private readonly TItem _new = n;

    /// <summary>
    /// Gets the old.
    /// </summary>
    /// <value>
    /// The old.
    /// </value>
    public readonly TItem Old => _old;

    /// <summary>
    /// Gets the new.
    /// </summary>
    /// <value>
    /// The new.
    /// </value>
    public readonly TItem New => _new;
}