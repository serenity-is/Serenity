using Serenity.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;

namespace Serenity.CodeGenerator
{
    public class SqliteSchemaProvider : ISchemaProvider
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
            var fields = connection.Query("PRAGMA table_info([" + table + "])")
                .Where(x => (int)x.pk > 0);

            if (fields.Count() == 1 &&
                (string)fields.First().type == "INTEGER")
            {
                return new List<string> { (string)fields.First().name };
            };

            return new List<string> { "ROWID" };
        }

        public IEnumerable<string> GetPrimaryKeyFields(IDbConnection connection, string schema, string table)
        {
            return connection.Query("PRAGMA table_info([" + table + "])")
                .Where(x => (int)x.pk > 0)
                .OrderBy(x => (int)x.pk)
                .Select(x => (string)x.name);
        }

        public IEnumerable<TableName> GetTableNames(IDbConnection connection)
        {
            return connection.Query(
                    "SELECT name, type FROM sqlite_master WHERE type='table' or type='view' " +
                    "ORDER BY name")
                .Select(x => new TableName
                {
                    Table = x.name,
                    IsView = x.type == "view"
                });
        }
    }
}