using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Serenity.Data.Schema
{
    /// <summary>
    /// SQLite metadata provider
    /// </summary>
    /// <seealso cref="ISchemaProvider" />
    public class SqliteSchemaProvider : ISchemaProvider
    {
        /// <summary>
        /// Gets the default schema.
        /// </summary>
        /// <value>
        /// The default schema.
        /// </value>
        public string DefaultSchema => null;

        /// <summary>
        /// Gets the field infos.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public IEnumerable<FieldInfo> GetFieldInfos(IDbConnection connection, string schema, string table)
        {
            return connection.Query("PRAGMA table_info([" + table + "])")
                .Select(x => new FieldInfo
                {
                    FieldName = x.name,
                    DataType = x.type,
                    IsNullable = Convert.ToInt32(x.notnull) != 1,
                    IsPrimaryKey = Convert.ToInt32(x.pk) == 1
                });
        }

        /// <summary>
        /// Gets the foreign keys.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public IEnumerable<ForeignKeyInfo> GetForeignKeys(IDbConnection connection, string schema, string table)
        {
            return connection.Query("PRAGMA foreign_key_list([" + table + "])")
                .Select(x => new ForeignKeyInfo
                {
                    FKName = x.id.ToString(),
                    FKColumn = x.from,
                    PKTable = x.table,
                    PKColumn = x.to
                });
        }

        /// <summary>
        /// Gets the identity fields.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the primary key fields.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public IEnumerable<string> GetPrimaryKeyFields(IDbConnection connection, string schema, string table)
        {
            return connection.Query("PRAGMA table_info([" + table + "])")
                .Where(x => (int)x.pk > 0)
                .OrderBy(x => (int)x.pk)
                .Select(x => (string)x.name);
        }

        /// <summary>
        /// Gets the table names.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
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