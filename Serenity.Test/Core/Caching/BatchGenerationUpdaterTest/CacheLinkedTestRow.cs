
namespace Serenity.Test.Core.Caching.BatchGenerationUpdaterTest
{
    using Serenity.Data;
    using Serenity.Data.Mapping;
    using System.ComponentModel;

    public class CacheLinkedTestRow : Row, IIdRow
    {
        protected int? _TestId;

        [DisplayName("ID"), PrimaryKey]
        public int? TestId
        {
            get { return _TestId; }
            set { Fields.TestId[this] = value; }
        }

        protected string _Name;

        [DisplayName("Name"), NotNull, Size(50)]
        public string Name
        {
            get { return _Name; }
            set { Fields.Name[this] = value; }
        }

        IIdField IIdRow.IdField
        {
            get { return Fields.TestId; }
        }

        public class RowFields : RowFieldsBase
        {
            public RowFields() : base("CacheLinkedTests") { }

            public Int32Field TestId;
            public StringField Name;
        }

        public static readonly RowFields Fields = new RowFields();

        public CacheLinkedTestRow()
            : base(Fields)
        {
        }

        public override Row CreateNew()
        {
            return new CacheLinkedTestRow();
        }
    }
}