using Newtonsoft.Json.Linq;

namespace Serenity.Data;

/// <summary>
/// Base class for fields with a value type value
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <seealso cref="Field" />
/// <seealso cref="IEnumTypeField" />
public abstract class GenericValueField<TValue> : Field, IEnumTypeField where TValue : struct, IComparable<TValue>
{
    /// <summary>
    /// The get value
    /// </summary>
    protected internal Func<IRow, TValue?> _getValue;
    /// <summary>
    /// The set value
    /// </summary>
    protected internal Action<IRow, TValue?> _setValue;
    /// <summary>
    /// The enum type
    /// </summary>
    protected internal Type _enumType;

    internal GenericValueField(ICollection<Field> collection, FieldType type, string name, LocalText caption, int size, FieldFlags flags,
        Func<IRow, TValue?> getValue = null, Action<IRow, TValue?> setValue = null)
        : base(collection, type, name, caption, size, flags)
    {
        _getValue = getValue ?? (r => (TValue?)(r.GetIndexedData(index)));
        _setValue = setValue ?? ((r, v) => r.SetIndexedData(index, v));
    }

    /// <summary>
    /// Converts the value.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="provider">The provider.</param>
    /// <returns></returns>
    public override object ConvertValue(object source, IFormatProvider provider)
    {
        if (source is JValue jValue)
            source = jValue.Value;

        if (source == null)
            return null;
        else
        {
            if (source is TValue val)
                return val;

            return Convert.ChangeType(source, typeof(TValue), provider);
        }
    }

    /// <summary>
    /// Gets or sets the type of the enum.
    /// </summary>
    /// <value>
    /// The type of the enum.
    /// </value>
    public Type EnumType
    {
        get { return _enumType; }
        set { _enumType = value; }
    }

    /// <summary>
    /// Copies the specified source.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="target">The target.</param>
    public override void Copy(IRow source, IRow target)
    {
        _setValue(target, _getValue(source));
        if (target.TrackAssignments)
            target.FieldAssignedValue(this);
    }

    /// <summary>
    /// Gets or sets the value of this field with the specified row.
    /// </summary>
    /// <param name="row">The row.</param>
    public TValue? this[IRow row]
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

    /// <summary>
    /// Gets the value of this field in specified row as object.
    /// </summary>
    /// <param name="row">The row.</param>
    public override object AsObject(IRow row)
    {
        return _getValue(row);
    }

    /// <summary>
    /// Sets the value of this field in specified row as object.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="value">The value.</param>
    /// <exception cref="InvalidCastException">Invalid cast exception while trying to set the value of {Name} field on {row.GetType().Name} as object.</exception>
    public override void AsObject(IRow row, object value)
    {
        if (value == null)
            _setValue(row, null);
        else
        {
            try
            {
                _setValue(row, (TValue)value);
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException($"Invalid cast exception while trying to set the value of {Name} field on {row.GetType().Name} as object.", ex);
            }
        }

        row.FieldAssignedValue(this);
    }

    /// <summary>
    /// Gets if the field value is null.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns></returns>
    protected override bool GetIsNull(IRow row)
    {
        return _getValue(row) == null;
    }

    /// <summary>
    /// Compares the field values for two rows for an ascending index sort
    /// </summary>
    /// <param name="row1">The row1.</param>
    /// <param name="row2">The row2.</param>
    /// <returns></returns>
    public override int IndexCompare(IRow row1, IRow row2)
    {
        var val1 = _getValue(row1);
        if (val1.HasValue)
        {
            var val2 = _getValue(row2);
            return val2.HasValue ? val1.Value.CompareTo(val2.Value) : 1;
        }
        else
            return _getValue(row2).HasValue ? -1 : 0;
    }

    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    /// <value>
    /// The type of the value.
    /// </value>
    public override Type ValueType => typeof(TValue?);
}