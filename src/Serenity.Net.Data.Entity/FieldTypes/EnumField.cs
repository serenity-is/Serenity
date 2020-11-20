using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    public class EnumField<TEnum> : Int32Field
        where TEnum : struct, IComparable, IFormattable, IConvertible
    {
        public EnumField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
            Func<IRow, int?> getValue = null, Action<IRow, int?> setValue = null)
            : base(collection, name, caption, size, flags, getValue, setValue)
        {
            if (!typeof(TEnum).IsEnum)
                throw new InvalidProgramException(typeof(TEnum).FullName + " is used as type parameter for an EnumField but it is not an enum type!");

            if (Enum.GetUnderlyingType(typeof(TEnum)) != typeof(int))
                throw new InvalidProgramException(typeof(TEnum).FullName + " is used as type parameter for an EnumField but it is not based on Int32!");

            EnumType = typeof(TEnum);
        }

        public new TEnum? this[IRow row]
        {
            get
            {
                CheckUnassignedRead(row);
                var value = _getValue(row);
                if (value == null)
                    return null;

                return (TEnum)(object)value;
            }
            set
            {
                int? v = value == null ? (int?)null : Convert.ToInt32(value);
                _setValue(row, v);
                row.FieldAssignedValue(this);
            }
        }
    }
}
