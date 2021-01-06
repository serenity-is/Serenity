
namespace Serenity.Test.Data
{
    using Serenity.Data;

    public partial class RowMappingTests
    {
        public class FaultyRow : Row
        {
            public string Ok
            {
                get { return Fields.Ok[this]; }
                set { Fields.Ok[this] = value; }
            }

            public class RowFields : RowFieldsBase
            {
                public StringField Ok;
                public StringField Missing;

                public RowFields()
                    : base("FaultyTable")
                {
                }
            }

            public static RowFields Fields = new RowFields().Init();

            public FaultyRow()
                : base(Fields)
            {
            }
        }
    }
}