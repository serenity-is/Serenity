namespace Serenity.Tests.Services;

public partial class MasterDetailRelationBehaviorTests
{
    public class Int32MasterRow : Row<Int32MasterRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID
        {
            get { return fields.ID[this]; }
            set { fields.ID[this] = value; }
        }

        [NotNull]
        public string Name
        {
            get { return fields.Name[this]; }
            set { fields.Name[this] = value; }
        }

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [MasterDetailRelation(foreignKey: "MasterID")]
        public List<Int32DetailRow> DetailList
        {
            get { return fields.DetailList[this]; }
            set { fields.DetailList[this] = value; }
        }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID;
            public StringField Name;
            public RowListField<Int32DetailRow> DetailList;
        }
    }
}