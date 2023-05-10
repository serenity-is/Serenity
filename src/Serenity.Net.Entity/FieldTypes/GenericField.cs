namespace Serenity.Data;

/// <summary>
/// Base generic class for fields with a value
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <seealso cref="Field" />
public abstract class GenericField<TValue> : Field
{
    /// <summary>
    /// The get value
    /// </summary>
    protected internal Func<IRow, TValue> _getValue;
    /// <summary>
    /// The set value
    /// </summary>
    protected internal Action<IRow, TValue> _setValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericField{TValue}"/> class.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="type">The type.</param>
    /// <param name="name">The name.</param>
    /// <param name="caption">The caption.</param>
    /// <param name="size">The size.</param>
    /// <param name="flags">The flags.</param>
    /// <param name="getValue">The get value.</param>
    /// <param name="setValue">The set value.</param>
    public GenericField(ICollection<Field> collection, FieldType type, string name, string caption, int size, FieldFlags flags,
        Func<IRow, TValue> getValue, Action<IRow, TValue> setValue)
        : base(collection, type, name, caption, size, flags)
    {
        _getValue = getValue;
        _setValue = setValue;
    }

    /// <summary>
    /// Copies the specified source.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="target">The target.</param>
    public override void Copy(IRow source, IRow target)
    {
        _setValue(target, _getValue(source));
        target.FieldAssignedValue(this);
    }

    /// <summary>
    /// Gets or sets the value of this field with the specified row.
    /// </summary>
    /// <param name="row">The row.</param>
    public TValue this[IRow row]
    {
        get
        {
            CheckUnassignedRead(row);
            return _getValue(row);
        }
        set
        {
            _setValue(row, value);
            row.FieldAssignedValue(this);
        }
    }
}