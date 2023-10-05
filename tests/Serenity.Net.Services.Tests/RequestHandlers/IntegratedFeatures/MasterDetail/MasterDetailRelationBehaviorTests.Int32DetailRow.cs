namespace Serenity.Tests.Services;

public partial class MasterDetailRelationBehaviorTests
{
    public class Int32DetailRow : Row<Int32DetailRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? DetailID { get => fields.DetailID[this]; set => fields.DetailID[this] = value; }

        [NotNull]
        public int? MasterID { get => fields.MasterID[this]; set => fields.MasterID[this] = value; }

        [NotNull]
        public int? ProductID { get => fields.ProductID[this]; set => fields.ProductID[this] = value; }

        [NotNull]
        public decimal? Quantity { get => fields.Quantity[this]; set => fields.Quantity[this] = value; }
        
        public class RowFields : RowFieldsBase
        {
            public Int32Field DetailID;
            public Int32Field MasterID;
            public Int32Field ProductID;
            public DecimalField Quantity;
        }
    }
}