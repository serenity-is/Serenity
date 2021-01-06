using Serenity.Data;
using Serenity.Data.Mapping;
using System;
using System.Collections.Generic;

namespace Serenity.Test.Services
{
    public partial class MasterDetailRelationBehaviorTests
    {
        [ConnectionKey("Serenity")]
        public class Int32MasterRow : Row, IIdRow
        {
            [Identity]
            public Int32? ID
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

            [NotMapped, MinSelectLevel(SelectLevel.Details)]
            [MasterDetailRelation(foreignKey: "MasterID")]
            public List<Int32DetailRow> DetailList
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

            public Int32MasterRow()
                : base(Fields)
            {
            }

            public class RowFields : RowFieldsBase
            {
                public Int32Field ID;
                public StringField Name;
                public RowListField<Int32DetailRow> DetailList;

                public RowFields()
                    : base("Int32Master")
                {
                }
            }
        }
    }
}