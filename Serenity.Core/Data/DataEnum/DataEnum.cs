using System;

namespace Serenity.Data
{
    public abstract class DataEnum : IEquatable<DataEnum>
    {
        internal Int32 _valueId;
        internal string _valueKey;

        protected DataEnum(string valueKey)
        {
            if (valueKey == null)
                throw new ArgumentNullException("valueKey");

            _valueKey = valueKey;
            _valueId = -1;
        }

        protected DataEnum(Int32 valueId)
        {
            _valueId = valueId;
            _valueKey = null;
        }

        protected abstract string GetEnumType();

        public string EnumType
        {
            get { return GetEnumType(); }
        }

        public void EnsureValid()
        {
            if (!IsValid)
            {
                if (_valueKey == null)
                    throw InvalidKeyException();
                else
                    throw InvalidIdException();
            }
        }

        public bool IsValid
        {
            get
            {
                return
                    (_valueKey != null && DataEnumCache.EnumType(GetEnumType())[_valueKey] != null) ||
                    (_valueKey == null && DataEnumCache.EnumType(GetEnumType())[_valueId] != null);
            }
        }

        public static DataEnum CreateDataEnum(Type enumType, Int32 value)
        {
            return (DataEnum)Activator.CreateInstance(enumType, new object[] { value });
        }

        public static DataEnum ConvertFromInt32(Type objectType, Int32 value)
        {
            DataEnum result = CreateDataEnum(objectType, value);
            if (!result.IsValid)
                throw result.InvalidIdException();

            return result;
        }

        private Exception InvalidKeyException()
        {
            return new InvalidOperationException(String.Format(
                "DataEnum tipi {0} için {1} kodlu değer bulunamadı!", GetEnumType(), _valueKey));
        }

        private Exception InvalidIdException()
        {
            throw new InvalidOperationException(String.Format(
                "DataEnum tipi {0} için {1} numaralı değer bulunamadı!", GetEnumType(), _valueId));
        }

        public static DataEnum ConvertFromString(Type objectType, String value)
        {
            value = value.TrimToNull();
            if (value == null)
                return null;

            Int32 v;
            if (Int32.TryParse(value, out v))
                return ConvertFromInt32(objectType, v);

            DataEnum result = (DataEnum)Activator.CreateInstance(objectType, new object[] { value });

            if (!result.IsValid)
                throw result.InvalidKeyException();

            return result;
        }

        public string Key
        {
            get
            {
                if (_valueKey != null)
                    return _valueKey;

                var key = DataEnumCache.EnumType(GetEnumType())[_valueId];
                if (key == null)
                    throw InvalidIdException();

                return key;
            }
        }

        public Int32 Id
        {
            get
            {
                if (_valueKey == null)
                    return _valueId;

                var valueId = DataEnumCache.EnumType(GetEnumType())[_valueKey];
                if (valueId == null)
                    throw InvalidKeyException();
                return valueId.Value;
            }
        }

        public static bool operator !=(DataEnum l, DataEnum r)
        {
            if (Object.ReferenceEquals(l, null))
                return !Object.ReferenceEquals(r, null);
            else if (Object.ReferenceEquals(r, null))
                return true;

            return !l.Equals(r);
        }

        public override int GetHashCode()
        {
            if (_valueKey == null)
                return _valueId.GetHashCode();
            else
                return _valueKey.GetHashCode();
        }

        public override string ToString()
        {
            if (IsValid)
                return Key;
            else
                return _valueKey ?? _valueId.ToInvariant();
        }

        public static bool operator ==(DataEnum l, DataEnum r)
        {
            if (Object.ReferenceEquals(l, null))
                return Object.ReferenceEquals(r, null);
            else if (Object.ReferenceEquals(r, null))
                return false;

            return l.Equals(r);
        }

        public override bool Equals(object obj)
        {
            return (obj is DataEnum) && Equals((DataEnum)obj);
        }

        public bool Equals(DataEnum other)
        {
            if (Object.ReferenceEquals(other, null))
                return false;

            if (other.GetEnumType() !=
                this.GetEnumType())
                return false;

            if (this._valueKey != null &&
                other._valueKey != null)
                return this._valueKey == other._valueKey;

            if (this._valueKey == null &&
                other._valueKey == null)
                return this._valueId == other._valueId;

            if (this.IsValid &&
                other.IsValid)
                return this.Id == other.Id;

            return false;
        }
    }

    public static class DataEnumExtensions
    {
        public static T Validate<T>(this T value) where T: DataEnum
        {
            value.EnsureValid();
            return value;
        }

        public static Int32? ToInt32(this DataEnum value)
        {
            if (value == null)
                return null;
            else
                return value.Id;
        }
    }
}