namespace Serenity.Tests.Services;

public partial class MasterDetailRelationBehaviorTests
{
    public class GuidMasterRow : Row<GuidMasterRow.RowFields>, IIdRow
    {
        [AutoIncrement, NotNull, PrimaryKey, IdProperty]
        public Guid? ID
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
        public List<GuidDetailRow> DetailList
        {
            get { return fields.DetailList[this]; }
            set { fields.DetailList[this] = value; }
        }

        public GuidMasterRow()
            : base(Fields)
        {
        }

        public class RowFields : RowFieldsBase
        {
            public GuidField ID;
            public StringField Name;
            public RowListField<GuidDetailRow> DetailList;
        }
    }
}