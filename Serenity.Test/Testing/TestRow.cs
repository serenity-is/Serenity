using System;
using Serenity.Data;
using Newtonsoft.Json;

namespace Serenity.Data
{
    using R = TestRow;

    ///   Entity class for a "Tests" table</summary>
    [JsonConverter(typeof(JsonRowConverter))]
    public sealed class TestRow : Row, IIdRow
    {
        /// <summary>Table name</summary>
        public const string TableName = "Tests";

        public class RowFields : RowFieldsBase
        {
            /// <summary>TestId field</summary>
            public readonly Int32Field TestId;
            /// <summary>Name field</summary>
            public readonly StringField Name;
            /// <summary>SomeId field</summary>
            public readonly Int32Field SomeId;
            /// <summary>Description field</summary>
            public readonly StringField Description;
            /// <summary>DisplayOrder field</summary>
            public readonly Int32Field DisplayOrder;

            public RowFields()
                : base(R.TableName, "")
            {
                TestId = new Int32Field(this, "TestId", "ID", 0, FieldFlags.Identity,
                    r => ((R)r)._TestId, (r, v) => ((R)r)._TestId = v);

                Name = new StringField(this, "Name", "Enum Tipi", 30, FieldFlags.Required,
                    r => ((R)r)._Name, (r, v) => ((R)r)._Name = v);

                SomeId = new Int32Field(this, "SomeId", "Değer No", 0, FieldFlags.Required,
                    r => ((R)r)._SomeId, (r, v) => ((R)r)._SomeId = v);

                Description = new StringField(this, "Description", "Değer Kodu", 30, FieldFlags.Required,
                    r => ((R)r)._Description, (r, v) => ((R)r)._Description = v);

                DisplayOrder = new Int32Field(this, "DisplayOrder", "Gösterim Sırası", 0, FieldFlags.Required,
                    r => ((R)r)._DisplayOrder, (r, v) => ((R)r)._DisplayOrder = v);
            }
        }

        /// <summary>Row fields</summary>
        public static readonly RowFields Fields;
        /// <summary>Shared instance of the row class</summary>
        public static readonly TestRow Instance;

        /// <summary>Initializes field definitions</summary>
        static TestRow()
        {
            Fields = new RowFields();
            Instance = new TestRow();
        }

        private Int32? _TestId;
        private String _Name;
        private Int32? _SomeId;
        private String _Description;
        private Int32? _DisplayOrder;

        /// <summary>Creates a new TestRow instance.</summary>
        public TestRow()
            : base(Fields)
        {
        }

        /// <summary>Creates a new TestRow instance.</summary>
        public override Row CreateNew()
        {
            return new TestRow();
        }

        /// <summary>TestId field value</summary>
        public Int32? TestId
        {
            get { return _TestId; }
            set { Fields.TestId[this] = value; }
        }

        /// <summary>Name field value</summary>
        public String Name
        {
            get { return _Name; }
            set { Fields.Name[this] = value; }
        }

        /// <summary>SomeId field value</summary>
        public Int32? SomeId
        {
            get { return _SomeId; }
            set { Fields.SomeId[this] = value; }
        }

        /// <summary>Description field value</summary>
        public String Description
        {
            get { return _Description; }
            set { Fields.Description[this] = value; }
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
            get { return Fields.TestId; }
        }
    }
}