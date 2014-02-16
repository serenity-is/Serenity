using System;
using Serenity.Data;
using Newtonsoft.Json;
using Xunit;
using Serenity.Data.Mapping;

namespace Serenity.Data
{
    public partial class RowMappingTests
    {
        public class InvalidColumnRow : Row
        {
            [Column("OverridenName")]
            public string Override
            {
                get { return Fields.Override[this]; }
                set { Fields.Override[this] = value; }
            }

            public class RowFields : RowFieldsBase
            {
                public StringField Override;

                public RowFields()
                    : base("InvalidColumn")
                {
                    Override = new StringField(this, "ManualName");
                }
            }

            public static RowFields Fields = new RowFields();
            public static InvalidColumnRow Instance = new InvalidColumnRow();

            public InvalidColumnRow()
                : base(Fields)
            {
            }

            public override Row CreateNew()
            {
                return new InvalidColumnRow();
            }
        }
    }
}