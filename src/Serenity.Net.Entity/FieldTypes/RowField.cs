namespace Serenity.Data;

/// <summary>
/// Field with a Row value
/// </summary>
/// <typeparam name="TForeign">The type of the foreign.</typeparam>
/// <seealso cref="CustomClassField{TForeign}" />
[NotMapped]
public class RowField<TForeign> : CustomClassField<TForeign> where TForeign : class, IRow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RowField{TForeign}"/> class.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="name">The name.</param>
    /// <param name="caption">The caption.</param>
    /// <param name="size">The size.</param>
    /// <param name="flags">The flags.</param>
    /// <param name="getValue">The get value.</param>
    /// <param name="setValue">The set value.</param>
    public RowField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default | FieldFlags.NotMapped,
        Func<IRow, TForeign> getValue = null, Action<IRow, TForeign> setValue = null)
        : base(collection, name, caption, size, flags, getValue, setValue)
    {
    }

    /// <summary>
    /// Compares the values.
    /// </summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns></returns>
    protected override int CompareValues(TForeign value1, TForeign value2)
    {
        if (value1 == null && value2 == null)
            return 0;

        if (value1 == null)
            return -1;

        if (value2 == null)
            return 1;

        foreach (var f in value1.Fields)
        {
            var c = f.IndexCompare(value1, value2);
            if (c != 0)
                return c;
        }

        return 0;
    }

    /// <summary>
    /// Clones the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    protected override TForeign Clone(TForeign value)
    {
        if (value == null)
            return null;

        return value.Clone();
    }
}