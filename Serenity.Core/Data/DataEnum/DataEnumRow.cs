using System;
using Serenity.Data;
using Newtonsoft.Json;

namespace Serenity.Data
{
    using R = DataEnumRow;

    /// <summary>
    ///   Entity class for "DataEnums" table</summary>
    [JsonConverter(typeof(JsonRowConverter))]
    public sealed class DataEnumRow : Row, IIdRow
    {
        /// <summary>Table name</summary>
        public const string TableName = "DataEnums";

        public class RowFields : RowFieldsBase
        {
            /// <summary>DataEnumId field</summary>
            public readonly Int32Field DataEnumId;
            /// <summary>EnumType field</summary>
            public readonly StringField EnumType;
            /// <summary>ValueId field</summary>
            public readonly Int32Field ValueId;
            /// <summary>ValueKey field</summary>
            public readonly StringField ValueKey;
            /// <summary>DisplayOrder field</summary>
            public readonly Int32Field DisplayOrder;

            public RowFields()
                : base(R.TableName, "")
            {
                DataEnumId = new Int32Field(this, "DataEnumId", "ID", 0, FieldFlags.Identity,
                    r => ((R)r)._DataEnumId, (r, v) => ((R)r)._DataEnumId = v);

                EnumType = new StringField(this, "EnumType", "Enum Tipi", 30, FieldFlags.Required,
                    r => ((R)r)._EnumType, (r, v) => ((R)r)._EnumType = v);

                ValueId = new Int32Field(this, "ValueId", "Değer No", 0, FieldFlags.Required,
                    r => ((R)r)._ValueId, (r, v) => ((R)r)._ValueId = v);

                ValueKey = new StringField(this, "ValueKey", "Değer Kodu", 30, FieldFlags.Required,
                    r => ((R)r)._ValueKey, (r, v) => ((R)r)._ValueKey = v);

                DisplayOrder = new Int32Field(this, "DisplayOrder", "Gösterim Sırası", 0, FieldFlags.Required,
                    r => ((R)r)._DisplayOrder, (r, v) => ((R)r)._DisplayOrder = v);

            }
        }

        /// <summary>Row fields</summary>
        public static readonly RowFields Fields;
        /// <summary>Shared instance of the row class</summary>
        public static readonly DataEnumRow Instance;

        /// <summary>Initializes field definitions</summary>
        static DataEnumRow()
        {
            Fields = new RowFields();
            Instance = new DataEnumRow();
        }

        private Int32? _DataEnumId;
        private String _EnumType;
        private Int32? _ValueId;
        private String _ValueKey;
        private Int32? _DisplayOrder;

        /// <summary>Creates a new DataEnumRow instance.</summary>
        public DataEnumRow()
            : base(Fields)
        {
        }

        /// <summary>Creates a new DataEnumRow instance.</summary>
        public override Row CreateNew()
        {
            return new DataEnumRow();
        }

        /// <summary>DataEnumId field value</summary>
        public Int32? DataEnumId
        {
            get { return _DataEnumId; }
            set { Fields.DataEnumId[this] = value; }
        }

        /// <summary>EnumType field value</summary>
        public String EnumType
        {
            get { return _EnumType; }
            set { Fields.EnumType[this] = value; }
        }

        /// <summary>ValueId field value</summary>
        public Int32? ValueId
        {
            get { return _ValueId; }
            set { Fields.ValueId[this] = value; }
        }

        /// <summary>ValueKey field value</summary>
        public String ValueKey
        {
            get { return _ValueKey; }
            set { Fields.ValueKey[this] = value; }
        }

        /// <summary>DisplayOrder field value</summary>
        public Int32? DisplayOrder
        {
            get { return _DisplayOrder; }
            set { Fields.DisplayOrder[this] = value; }
        }

        /// <summary>Implements IIdRow.IdField by returning the identity field</summary>
        IIdField IIdRow.IdField
        {
            get { return Fields.DataEnumId; }
        }
    }
}