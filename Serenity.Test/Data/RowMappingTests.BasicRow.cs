using System;
using Serenity.Data;
using Newtonsoft.Json;
using Xunit;

namespace Serenity.Data
{
    public partial class RowMappingTests
    {
        public class BasicRow : Row
        {
            public string AString
            {
                get { return Fields.AString[this]; }
                set { Fields.AString[this] = value; }
            }

            public Int32? AInt32
            {
                get { return Fields.AInt32[this]; }
                set { Fields.AInt32[this] = value; }
            }

            public class RowFields : RowFieldsBase
            {
                public StringField AString;
                public Int32Field AInt32;
            }

            public static RowFields Fields = new RowFields();
            public static BasicRow Instance = new BasicRow();

            public BasicRow()
                : base(Fields)
            {
            }

            public override Row CreateNew()
            {
                return new BasicRow();
            }
        }
    }
}