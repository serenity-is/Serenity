using Serenity.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Serenity.CodeGenerator
{
    public class FirebirdSchemaProvider : ISchemaProvider
    {
        public string DefaultSchema { get { return null; } }

        public IEnumerable<FieldInfo> GetFieldInfos(IDbConnection connection, string schema, string table)
        {
            return new List<FieldInfo>();
        }

        public IEnumerable<ForeignKeyInfo> GetForeignKeys(IDbConnection connection, string schema, string table)
        {
            return connection.Query<ForeignKeyInfo>(@"
                SELECT
                    PK.RDB$RELATION_NAME as PKTable,
                    ISP.RDB$FIELD_NAME as PKColumn,
                    FK.RDB$CONSTRAINT_NAME as FKName
                FROM
                    RDB$RELATION_CONSTRAINTS PK, 
                    RDB$RELATION_CONSTRAINTS FK, 
                    RDB$REF_CONSTRAINTS RC, 
                    RDB$INDEX_SEGMENTS ISP, 
                    RDB$INDEX_SEGMENTS ISF
                    WHERE FK.RDB$RELATION_NAME = @tbl
                    AND FK.RDB$CONSTRAINT_NAME = RC.RDB$CONSTRAINT_NAME
                    AND PK.RDB$CONSTRAINT_NAME = RC.RDB$CONST_NAME_UQ
                    AND ISP.RDB$INDEX_NAME = PK.RDB$INDEX_NAME
                    AND ISF.RDB$INDEX_NAME = FK.RDB$INDEX_NAME
                    AND ISP.RDB$FIELD_POSITION = ISF.RDB$FIELD_POSITION
                    ORDER BY ISP.RDB$FIELD_POSITION", new
            {
                tbl = table
            }).Select(x =>
            {
                x.FKName = x.FKName.TrimToNull();
                x.FKColumn = x.FKColumn.TrimToNull();
                x.PKColumn = x.PKColumn.TrimToNull();
                x.PKTable = x.PKTable.TrimToNull();
                return x;
            });
        }

        public IEnumerable<string> GetIdentityFields(IDbConnection connection, string schema, string table)
        {
            var match = connection.Query<string>(@"
                    SELECT RDB$GENERATOR_NAME
                    FROM RDB$GENERATORS
                    WHERE RDB$SYSTEM_FLAG = 0 AND RDB$GENERATOR_NAME LIKE @genprefix",
                new
                {
                    genprefix = "GEN_" + table + "_%"
                })
                .Select(x => x.Substring(("GEN_" + table + "_").Length))
                .FirstOrDefault()
                .TrimToNull();

            if (match == null)
            {
                var primaryKeys = this.GetPrimaryKeyFields(connection, schema, table);
                if (primaryKeys.Count() == 1)
                    return primaryKeys;

                return new List<string>();
            }

            return connection.Query<string>(@"
                    SELECT RDB$FIELD_NAME
                    FROM RDB$RELATION_FIELDS
                    WHERE RDB$RELATION_NAME = @tbl
                    AND RDB$FIELD_NAME LIKE @match
                    ORDER BY RDB$FIELD_POSITION",
                new
                {
                    tbl = table,
                    match = match + "%"
                })
                .Take(1)
                .Select(StringHelper.TrimToNull);
        }

        public IEnumerable<string> GetPrimaryKeyFields(IDbConnection connection, string schema, string table)
        {
            return connection.Query<string>(@"
                SELECT ISGMT.RDB$FIELD_NAME FROM
                RDB$RELATION_CONSTRAINTS rc
                INNER JOIN RDB$INDEX_SEGMENTS ISGMT ON rc.RDB$INDEX_NAME = ISGMT.RDB$INDEX_NAME
                WHERE CAST(RC.RDB$RELATION_NAME AS VARCHAR(40)) = @tbl 
                    AND RC.RDB$CONSTRAINT_TYPE = 'PRIMARY KEY'
                ORDER BY ISGMT.RDB$FIELD_POSITION", new { tbl = table })
                    .Select(StringHelper.TrimToNull);
        }

        public IEnumerable<TableName> GetTableNames(IDbConnection connection)
        {
            return connection.Query(@"
                    SELECT RDB$RELATION_NAME NAME, RDB$VIEW_BLR ISVIEW
                    FROM RDB$RELATIONS  
                    WHERE (RDB$SYSTEM_FLAG IS NULL OR RDB$SYSTEM_FLAG = 0)")
                .Select(x => new TableName
                {
                    Table = StringHelper.TrimToNull(x.NAME),
                    IsView = x.ISVIEW != null
                });
        }
    }
}