using Serenity.Data;
using Serenity.Data.Mapping;
using System;

namespace Serenity.Test.Services
{
    public partial class MasterDetailRelationBehaviorTests
    {
        public class GuidDetailRow : Row, IIdRow
        {
            [AutoIncrement, PrimaryKey, NotNull]
            public Guid? DetailID
            {
                get { return Fields.DetailID[this]; }
                set { Fields.DetailID[this] = value; }
            }

            [NotNull]
            public Guid? MasterID
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

            public GuidDetailRow()
                : base(Fields)
            {
            }

            public class RowFields : RowFieldsBase
            {
                public GuidField DetailID;
                public GuidField MasterID;
                public Int32Field ProductID;
                public DecimalField Quantity;

                public RowFields()
                    : base("GuidDetail")
                {
                }
            }
        }
    }
}