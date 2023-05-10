namespace Serenity.Data;

/// <summary>
/// Field with a list value
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
[NotMapped]
public class ListField<TItem> : CustomClassField<List<TItem>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ListField{TItem}"/> class.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="name">The name.</param>
    /// <param name="caption">The caption.</param>
    /// <param name="size">The size.</param>
    /// <param name="flags">The flags.</param>
    /// <param name="getValue">The get value.</param>
    /// <param name="setValue">The set value.</param>
    public ListField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default | FieldFlags.NotMapped,
        Func<IRow, List<TItem>> getValue = null, Action<IRow, List<TItem>> setValue = null)
        : base(collection, name, caption, size, flags, getValue, setValue)
    {
    }

    /// <summary>
    /// Compares the values.
    /// </summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns></returns>
    protected override int CompareValues(List<TItem> value1, List<TItem> value2)
    {
        var comparer = Comparer<TItem>.Default;
        var length = Math.Min(value1.Count, value2.Count);
        for (var i = 0; i < length; i++)
        {
            var c = comparer.Compare(value1[i], value2[i]);
            if (c != 0)
                return c;
        }

        return value1.Count.CompareTo(value2.Count);
    }

    /// <summary>
    /// Clones the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    protected override List<TItem> Clone(List<TItem> value)
    {
        var clone = new List<TItem>();
        foreach (var item in value)
            clone.Add(item);
        return clone;
    }
}
