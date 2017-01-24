using Serenity.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Serenity.CodeGenerator
{
    public class MySqlSchemaProvider : ISchemaProvider
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
                    i.CONSTRAINT_NAME FKName,
                    k.COLUMN_NAME FKColumn, 
                    CASE WHEN k.REFERENCED_TABLE_SCHEMA = Database() THEN NULL 
                        ELSE k.REFERENCED_TABLE_SCHEMA END PKSchema, 
                    k.REFERENCED_TABLE_NAME PKTable,
                    k.REFERENCED_COLUMN_NAME PKColumn
                FROM information_schema.TABLE_CONSTRAINTS i
                LEFT JOIN information_schema.KEY_COLUMN_USAGE k ON i.CONSTRAINT_NAME = k.CONSTRAINT_NAME
                WHERE i.CONSTRAINT_TYPE = 'FOREIGN KEY'
                AND i.TABLE_SCHEMA = Database()
                AND i.TABLE_NAME = @tbl", new
            {
                tbl = table
            });
        }

        public IEnumerable<string> GetIdentityFields(IDbConnection connection, string schema, string table)
        {
            return connection.Query<string>(@"
                    SELECT COLUMN_NAME FROM information_schema.COLUMNS 
                    WHERE TABLE_SCHEMA = Database()
                    AND table_name = @tbl
                    AND EXTRA = 'auto_increment'", 
                new
                {
                    tbl = table
                });
        }

        public IEnumerable<string> GetPrimaryKeyFields(IDbConnection connection, string schema, string table)
        {
            return connection.Query<string>(@"
                    SELECT COLUMN_NAME  
                    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc  
                    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS ku  
                    USING(constraint_name,table_schema,table_name)
                    WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'  
                    AND tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME  
                    AND ku.TABLE_SCHEMA = Database()  
                    AND ku.TABLE_NAME = @tbl  
                    ORDER BY ku.ORDINAL_POSITION",
                new
                {
                    tbl = table
                });
        }

        public IEnumerable<TableName> GetTableNames(IDbConnection connection)
        {
            return connection.Query(
                    "SELECT TABLE_NAME, TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES " +
                    "WHERE TABLE_SCHEMA = Database() " +
                    "ORDER BY TABLE_SCHEMA, TABLE_NAME")
                .Select(x => new TableName
                {
                    Table = x.TABLE_NAME,
                    IsView = x.TABLE_TYPE == "VIEW"
                });
        }
    }
}