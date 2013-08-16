using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;

namespace Marmara.Data
{
    /// <summary>
    ///   A field that holds string values, but provides this value by reading an ID from an Int64 typed field and 
    ///   looking up it from an object implementing <see cref="IDbLookupCache"/> interface.</summary>
    /// <remarks>
    ///   This special field type is defined Bu özel alan tipi City, Gender gibi metinsel alanların CityID, GenderID gibi bağlı oldukları ID 
    ///   alanlarına göre, database'den sorgulanmadan direk bir önbellek üzerinden okunabilmelerine imkan 
    ///   sağlamak için tanımlanmıştır.</remarks>
    public class LookupField : StringField
    {
        private Int64Field _idField;
        private IDbLookupCache _idLookup;

        /// <summary>
        ///   Creates a new <c>LookupField</c> object.</summary>
        /// <param name="row">
        ///   Row (required)</param>
        /// <param name="meta">
        ///   Field field (required)</param>
        /// <param name="idField">
        ///   ID field this lookup field is based on (required, for example if this field is fCountry then the ID field
        ///   should be fCountryID)</param>
        /// <param name="idLookup">
        ///   An object that implements IDbLookupCache interface, which will be used to look up field value by
        ///   searching with ID (by using GetNameByID function).</param>

        public LookupField(ICollection<Field> collection, FieldMeta meta, Func<Row, String> getValue, Action<Row, String> setValue,
                Int64Field idField, IDbLookupCache idLookup)
            : base(collection, meta, getValue, setValue)
        {
            if (idField == null)
                throw new ArgumentNullException("idField");
            if (idLookup == null)
                throw new ArgumentNullException("idLookup");

            _idField = idField;
            _idLookup = idLookup;
        }

        /// <summary>
        ///   Compares two records for field values.</summary>
        /// <param name="compareTo">
        ///   Field object of the same type to compare field value to.</param>
        /// <param name="nullsAtTail">
        ///   If nulls should assumed to be at tail, true.</param>
        /// <returns>
        ///   If two field values are equal for ordering 0, if this is smaller then other &lt;0, else &gt;0</returns>
        public override int IndexCompare(Row row1, Row row2)
        {
            var value1 = this[row1];
            var value2 = this[row2];

            bool null1 = value1 == null;
            bool null2 = value2 == null;
            if (null1 || null2)
            {
                if (null1 && null2)
                    return 0;
                else if (null1)
                    return -1;
                else
                    return 1;
            }
            else
                return value1.CompareTo(value2);
        }

        /// <summary>
        ///   Gets/sets field value as <see cref="Object"/></summary>
        public override object AsObject(Row row)
        {
 	         return this[row];
        }

        /// <summary>
        ///   Gets if field value is null</summary>
        public override bool IsNull(Row row)
        {
            return this[row] == null;
        }

        /// <summary>
        ///   Gets/sets field value.</summary>
        public new string this[Row row]
        {
            get 
            {
                string value = base[row];
                
                if (value == null && !_idField.IsNull(row))
                    return _idLookup.GetNameByID(_idField[row].Value);

                return value;
            }
            set
            {
                _setValue(row, value);
                if (row._tracking)
                    row.FieldAssignedValue(this);
            }
        }


        /// <summary>
        ///   Gets the ID field that this field is related to.</summary>
        public Int64Field IDField
        {
            get { return _idField; }
        }

        /// <summary>
        ///   Gets the lookup cache that this field is related to.</summary>
        public IDbLookupCache IDLookup
        {
            get { return _idLookup; }
        }

        public override void ValueToJson(Newtonsoft.Json.JsonWriter writer, Row row, JsonSerializer serializer)
        {
            writer.WriteValue(_getValue(row));
        }
    }
}
