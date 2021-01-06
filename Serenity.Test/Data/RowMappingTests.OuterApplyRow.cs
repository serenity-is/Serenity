using Serenity.Data;
using Serenity.Data.Mapping;
using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Serenity.Test.Data
{
    public partial class RowMappingTests
    {
        [TableName("MainTable")]
        [OuterApply("outerTable", @"SELECT DISTINCT [outer_table].[column_id], [outer_table].[column_name] FROM [schema].[some_other_table] AS [outer_table]
	        WHERE jLeftJoinedTable.outer_table_related_id = outer_table.id")]
        public class OuterApplyRow : Row
        {
            [Identity]
            public Int32? ID
            {
                get { return Fields.ID[this]; }
                set { Fields.ID[this] = value; }
            }

            [Expression("outerTable.column_id")]
            public Int32? OuterColumnId
            {
                get { return Fields.OuterColumnId[this]; }
                set { Fields.OuterColumnId[this] = value; }
            }

            [Expression("outerTable.column_name")]
            public String OuterColumnName
            {
                get { return Fields.OuterColumnName[this]; }
                set { Fields.OuterColumnName[this] = value; }
            }

            [Column("left_joined_id"), ForeignKey("left_joined_table", "id"), LeftJoin("jLeftJoinedTable")]
            public Int32? LeftJoinedId
            {
                get { return Fields.LeftJoinedId[this]; }
                set { Fields.LeftJoinedId[this] = value; }
            }

            [Expression("jLeftJoinedTable.left_joined_column")]
            public String LeftJoinedColumn
            {
                get { return Fields.LeftJoinedColumn[this]; }
                set { Fields.LeftJoinedColumn[this] = value; }
            }

            public class RowFields : RowFieldsBase
            {
                public Int32Field ID;
                public Int32Field OuterColumnId;
                public StringField OuterColumnName;
                public Int32Field LeftJoinedId;
                public StringField LeftJoinedColumn;

                public RowFields()
                {
                }
            }

            public static RowFields Fields = new RowFields().Init();

            public OuterApplyRow()
                : base(Fields)
            {
            }
        }
    }
}