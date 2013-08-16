using System;
using System.Collections;
using System.ComponentModel;

namespace Serenity.Data
{
    internal sealed class FieldDescriptor : PropertyDescriptor
    {
        private Field _field;

        internal FieldDescriptor(Field field)
            : base(field.PropertyName, null)
        {
            _field = field;
        }

        public override bool CanResetValue(object component)
        {
            return component != null && !_field.IsNull((Row)component);
        }

        public override bool Equals(object other)
        {
            if (other is FieldDescriptor)
            {
                FieldDescriptor descriptor = (FieldDescriptor)other;

                return (descriptor._field == this._field);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _field.GetHashCode();
        }

        public override object GetValue(object component)
        {
            if (component == null)
                return null;

            var val = _field.AsObject((Row)component);
            if (val == null)
                return null;

            var enumType = GetEnumType(_field);

            if (enumType != null &&
                enumType.IsEnum)
                return Enum.ToObject(enumType, val);

            return val;
        }

        public override void ResetValue(object component)
        {
            if (component != null)
                _field.AsObject((Row)component, null);
        }

        public override void SetValue(object component, object value)
        {
            if (component == null)
                return;

            if (value != null &&
                _field.ValueType != value.GetType())
            {
                value = _field.ConvertValue(value, System.Globalization.CultureInfo.CurrentCulture);
            }
            _field.AsObject((Row)component, value);
            this.OnValueChanged(component, EventArgs.Empty);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override AttributeCollection Attributes
        {
            get
            {
                if (typeof(IList).IsAssignableFrom(this.PropertyType))
                {
                    Attribute[] array = new Attribute[base.Attributes.Count + 1];
                    base.Attributes.CopyTo(array, 0);
                    array[array.Length - 1] = new ListBindableAttribute(false);
                    return new AttributeCollection(array);
                }
                return base.Attributes;
            }
        }

        public override Type ComponentType
        {
            get
            {
                return _field._rowType ?? typeof(Row);
            }
        }

        public override bool IsBrowsable
        {
            get
            {
                return base.IsBrowsable;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public static Type GetEnumType(Field field)
        {
            var fint32 = field as Int32Field;
            if (fint32 != null &&
                fint32.EnumType != null &&
                fint32.EnumType.IsEnum)
            {
                return fint32.EnumType;
            }

            var fint16 = field as Int16Field;
            if (fint16 != null &&
                fint16.EnumType != null &&
                fint16.EnumType.IsEnum)
            {
                return fint16.EnumType;
            }

            return null;
        }

        public override Type PropertyType
        {
            get
            {
                var enumType = GetEnumType(_field);
                if (enumType != null &&
                    enumType.IsEnum)
                    return enumType;

                return _field.ValueType;                    
                    
            }
        }
    }
}