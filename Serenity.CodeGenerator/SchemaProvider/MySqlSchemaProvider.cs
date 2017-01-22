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
            return new List<ForeignKeyInfo>();
        }

        public IEnumerable<string> GetIdentityFields(IDbConnection connection, string schema, string table)
        {
            return new List<string>();
        }

        public IEnumerable<string> GetPrimaryKeyFields(IDbConnection connection, string schema, string table)
        {
            return new List<string>();
        }

        public IEnumerable<TableName> GetTableNames(IDbConnection connection)
        {
            return new List<TableName>();
        }
    }
}