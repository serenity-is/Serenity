namespace Serenity.Tests.Services;

public partial class MasterDetailRelationBehaviorTests
{
    public class GuidDetailRow : Row<GuidDetailRow.RowFields>, IIdRow
    {
        [AutoIncrement, PrimaryKey, NotNull, IdProperty]
        public Guid? DetailID
        {
            get { return fields.DetailID[this]; }
            set { fields.DetailID[this] = value; }
        }

        [NotNull]
        public Guid? MasterID
        {
            get { return fields.MasterID[this]; }
            set { fields.MasterID[this] = value; }
        }

        [NotNull]
        public int? ProductID
        {
            get { return fields.ProductID[this]; }
            set { fields.ProductID[this] = value; }
        }

        [NotNull]
        public decimal? Quantity
        {
            get { return fields.Quantity[this]; }
            set { fields.Quantity[this] = value; }
        }

        public class RowFields : RowFieldsBase
        {
            public GuidField DetailID;
            public GuidField MasterID;
            public Int32Field ProductID;
            public DecimalField Quantity;
        }
    }
}