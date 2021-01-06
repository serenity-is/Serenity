using Serenity.Data;
using Serenity.Data.Mapping;
using System;

namespace Serenity.Test.Services
{
    public partial class MasterDetailRelationBehaviorTests
    {
        public class Int32DetailRow : Row, IIdRow
        {
            [Identity]
            public Int32? DetailID
            {
                get { return Fields.DetailID[this]; }
                set { Fields.DetailID[this] = value; }
            }

            [NotNull]
            public Int32? MasterID
            {
                get { return Fields.MasterID[this]; }
                set { Fields.MasterID[this] = value; }
            }

            [NotNull]
            public Int32? ProductID
            {
                get { return Fields.ProductID[this]; }
                set { Fields.ProductID[this] = value; }
            }

            [NotNull]
            public Decimal? Quantity
            {
                get { return Fields.Quantity[this]; }
                set { Fields.Quantity[this] = value; }
            }

            public IIdField IdField
            {
                get
                {
                    return Fields.DetailID;
                }
            }

            public static RowFields Fields = new RowFields().Init();

            public Int32DetailRow()
                : base(Fields)
            {
            }

            public class RowFields : RowFieldsBase
            {
                public Int32Field DetailID;
                public Int32Field MasterID;
                public Int32Field ProductID;
                public DecimalField Quantity;

                public RowFields()
                    : base("Int32Detail")
                {
                }
            }
        }
    }
}