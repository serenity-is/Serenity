
namespace Serenity.Test.Data
{
    using System;
    using Serenity.Data;

    public partial class RowMappingTests
    {
        public class NamePropertyRow : Row
        {
            [NameProperty]
            public string AName
            {
                get { return Fields.AName[this]; }
                set { Fields.AName[this] = value; }
            }

            public Int32? AInt32
            {
                get { return Fields.AInt32[this]; }
                set { Fields.AInt32[this] = value; }
            }

            public class RowFields : RowFieldsBase
            {
                public StringField AName;
                public Int32Field AInt32;
            }

            public static RowFields Fields = new RowFields().Init();

            public NamePropertyRow()
                : base(Fields)
            {
            }
        }
    }
}