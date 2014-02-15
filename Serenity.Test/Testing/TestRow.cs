
namespace Serenity.Data
{
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel;

    [JsonConverter(typeof(JsonRowConverter))]
    public class TestRow : Row, IIdRow
    {
        protected Int32? _TestId;

        [DisplayName("ID"), PrimaryKey, AutoIncrement]
        public Int32? TestId
        {
            get { return _TestId; }
            set { Fields.TestId[this] = value; }
        }

        protected String _Name;

        [DisplayName("ID"), NotNull, Size(50)]
        public String Name
        {
            get { return _Name; }
            set { Fields.Name[this] = value; }
        }

        protected Int32? _SomeId;

        [DisplayName("Some ID"), NotNull]
        public Int32? SomeId
        {
            get { return _SomeId; }
            set { Fields.SomeId[this] = value; }
        }

        protected String _Description;

        [DisplayName("Description")]
        public String Description
        {
            get { return _Description; }
            set { Fields.Description[this] = value; }
        }

        protected Int32? _DisplayOrder;

        [DisplayName("Display Order"), NotNull]
        public Int32? DisplayOrder
        {
            get { return _DisplayOrder; }
            set { Fields.DisplayOrder[this] = value; }
        }

        IIdField IIdRow.IdField
        {
            get { return Fields.TestId; }
        }

        public class RowFields : RowFieldsBase
        {
            public RowFields() : base("Tests") { }

            public readonly Int32Field TestId;
            public readonly StringField Name;
            public readonly Int32Field SomeId;
            public readonly StringField Description;
            public readonly Int32Field DisplayOrder;
        }

        public static readonly RowFields Fields = new RowFields();

        public TestRow()
            : base(Fields)
        {
        }

        public override Row CreateNew()
        {
            return new TestRow();
        }
    }
}