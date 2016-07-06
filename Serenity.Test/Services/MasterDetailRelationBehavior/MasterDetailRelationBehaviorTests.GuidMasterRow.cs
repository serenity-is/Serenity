using Serenity.Data;
using Serenity.Data.Mapping;
using System;
using System.Collections.Generic;

namespace Serenity.Test.Services
{
    public partial class MasterDetailRelationBehaviorTests
    {
        [ConnectionKey("Serenity")]
        public class GuidMasterRow : Row, IIdRow
        {
            [AutoIncrement, NotNull, PrimaryKey]
            public Guid? ID
            {
                get { return Fields.ID[this]; }
                set { Fields.ID[this] = value; }
            }

            [NotNull]
            public String Name
            {
                get { return Fields.Name[this]; }
                set { Fields.Name[this] = value; }
            }

            [ClientSide, MinSelectLevel(SelectLevel.Details)]
            [MasterDetailRelation(foreignKey: "MasterID")]
            public List<GuidDetailRow> DetailList
            {
                get { return Fields.DetailList[this]; }
                set { Fields.DetailList[this] = value; }
            }

            public IIdField IdField
            {
                get
                {
                    return Fields.ID;
                }
            }

            public static RowFields Fields = new RowFields().Init();

            public GuidMasterRow()
                : base(Fields)
            {
            }

            public class RowFields : RowFieldsBase
            {
                public GuidField ID;
                public StringField Name;
                public RowListField<GuidDetailRow> DetailList;

                public RowFields()
                    : base("GuidMaster")
                {
                }
            }
        }
    }
}