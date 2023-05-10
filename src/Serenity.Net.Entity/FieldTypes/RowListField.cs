namespace Serenity.Data;

/// <summary>
/// Field with a RowList value
/// </summary>
/// <typeparam name="TForeign">The type of the foreign.</typeparam>
[NotMapped]
public class RowListField<TForeign> : CustomClassField<List<TForeign>> where TForeign : class, IRow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RowListField{TForeign}"/> class.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="name">The name.</param>
    /// <param name="caption">The caption.</param>
    /// <param name="size">The size.</param>
    /// <param name="flags">The flags.</param>
    /// <param name="getValue">The get value.</param>
    /// <param name="setValue">The set value.</param>
    public RowListField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default | FieldFlags.NotMapped,
        Func<IRow, List<TForeign>> getValue = null, Action<IRow, List<TForeign>> setValue = null)
        : base(collection, name, caption, size, flags, getValue, setValue)
    {
    }

    /// <summary>
    /// Compares the values.
    /// </summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns></returns>
    protected override int CompareValues(List<TForeign> value1, List<TForeign> value2)
    {
        var length = Math.Min(value1.Count, value2.Count);

        for (var i = 0; i < length; i++)
        {
            var v1 = value1[i];
            var v2 = value2[i];

            if (v1 == null && v2 == null)
                continue;

            if (v1 == null)
                return -1;

            if (v2 == null)
                return 1;

            foreach (var f in v1.Fields)
            {
                var c = f.IndexCompare(v1, v2);
                if (c != 0)
                    return c;
            }
        }

        return value1.Count.CompareTo(value2.Count);
    }

    /// <summary>
    /// Clones the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    protected override List<TForeign> Clone(List<TForeign> value)
    {
        var clone = new List<TForeign>();
        foreach (var row in value)
            clone.Add(row.Clone());
        return clone;
    }
}
