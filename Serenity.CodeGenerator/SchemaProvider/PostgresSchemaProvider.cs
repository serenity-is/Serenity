using Serenity.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Serenity.CodeGenerator
{
    public class PostgresSchemaProvider : ISchemaProvider
    {
        public string DefaultSchema { get { return "public"; } }

        public IEnumerable<FieldInfo> GetFieldInfos(IDbConnection connection, string schema, string table)
        {
            return new List<FieldInfo>();
        }

        public IEnumerable<ForeignKeyInfo> GetForeignKeys(IDbConnection connection, string schema, string table)
        {
            return new List<ForeignKeyInfo>();
        }

        public IEnumerable<string> GetIdentityFields(IDbConnection connection, string schema, string table)
        {
            return new List<string>();
        }

        public IEnumerable<string> GetPrimaryKeyFields(IDbConnection connection, string schema, string table)
        {
            return connection.Query<string>(
                "SELECT pg_attribute.attname " +
                "FROM pg_index, pg_class, pg_attribute, pg_namespace " +
                "WHERE pg_class.oid = '\"" + table + "\"'::regclass " +
                "AND indrelid = pg_class.oid " +
                "AND nspname = '" + schema + "' " +
                "AND pg_class.relnamespace = pg_namespace.oid " +
                "AND pg_attribute.attrelid = pg_class.oid " +
                "AND pg_attribute.attnum = any(pg_index.indkey) " +
                "AND indisprimary");
        }

        public IEnumerable<TableName> GetTableNames(IDbConnection connection)
        {
            return connection.Query(
                    "SELECT table_schema, table_name, table_type " + 
                    "FROM information_schema.tables " +
                    "WHERE table_schema NOT IN ('pg_catalog', 'information_schema') " +
                    "ORDER BY table_schema, table_name")
                .Select(x => new TableName
                {
                    Schema = x.table_schema,
                    Table = x.table_name,
                    IsView = x.table_type == "VIEW"
                });
        }
    }
}