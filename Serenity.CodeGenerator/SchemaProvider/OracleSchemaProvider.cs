using Serenity.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Serenity.CodeGenerator
{
    public class OracleSchemaProvider : ISchemaProvider
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
                    a.constraint_name FKName,                     
                    a.column_name FKColumn,
                    c.r_owner PKSchema,
                    c_pk.table_name PKTable,
                    uc.column_name PKColumn
                FROM all_cons_columns a
                JOIN all_constraints c ON a.owner = c.owner AND a.constraint_name = c.constraint_name
                JOIN all_constraints c_pk ON c.r_owner = c_pk.owner AND c.r_constraint_name = c_pk.constraint_name
                JOIN user_cons_columns uc ON uc.constraint_name = c.r_constraint_name
                WHERE c.constraint_type = 'R' 
                    AND a.table_name = :tbl
                    AND c.r_owner = :sma", new
            {
                sma = schema,
                tbl = table
            });
        }

        public IEnumerable<string> GetIdentityFields(IDbConnection connection, string schema, string table)
        {
            return new List<string>();
        }

        public IEnumerable<string> GetPrimaryKeyFields(IDbConnection connection, string schema, string table)
        {
            return connection.Query<string>(@"
                    SELECT cols.column_name
                    FROM all_constraints cons, all_cons_columns cols
                    WHERE cols.table_name = :tbl
                    AND cons.constraint_type = 'P'
                    AND cons.constraint_name = cols.constraint_name
                    AND cons.owner = :sch
                    ORDER BY cols.position;",
                new
                {
                    sch = schema,
                    tbl = table
                });
        }

        public IEnumerable<TableName> GetTableNames(IDbConnection connection)
        {
            return connection.Query("SELECT owner, table_name FROM all_tables")
                .Select(x => new TableName
                {
                    Schema = x.owner,
                    Table = x.table_name
                });
        }
    }
}