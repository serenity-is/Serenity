namespace Serenity.Tests.Services;

public partial class MasterDetailRelationBehaviorTests
{
    public class Int32DetailRow : Row<Int32DetailRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? DetailID
        {
            get { return Fields.DetailID[this]; }
            set { Fields.DetailID[this] = value; }
        }

        [NotNull]
        public int? MasterID
        {
            get { return Fields.MasterID[this]; }
            set { Fields.MasterID[this] = value; }
        }

        [NotNull]
        public int? ProductID
        {
            get { return Fields.ProductID[this]; }
            set { Fields.ProductID[this] = value; }
        }

        [NotNull]
        public decimal? Quantity
        {
            get { return Fields.Quantity[this]; }
            set { Fields.Quantity[this] = value; }
        }
        
        public class RowFields : RowFieldsBase
        {
            public Int32Field DetailID;
            public Int32Field MasterID;
            public Int32Field ProductID;
            public DecimalField Quantity;
        }
    }
}