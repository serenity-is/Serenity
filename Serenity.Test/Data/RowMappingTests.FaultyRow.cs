using System;
using Serenity.Data;
using Newtonsoft.Json;
using Xunit;

namespace Serenity.Data
{
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

            public static RowFields Fields = new RowFields();
            public static FaultyRow Instance = new FaultyRow();

            public FaultyRow()
                : base(Fields)
            {
            }

            public override Row CreateNew()
            {
                return new FaultyRow();
            }
        }
    }
}