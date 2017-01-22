using System.Collections.Generic;
using System.Data;

namespace Serenity.CodeGenerator
{
    public interface ISchemaProvider
    {
        string DefaultSchema { get; }
        IEnumerable<ForeignKeyInfo> GetForeignKeys(IDbConnection connection, string schema, string table);
        IEnumerable<string> GetIdentityFields(IDbConnection connection, string schema, string table);
        IEnumerable<string> GetPrimaryKeyFields(IDbConnection connection, string schema, string table);
        IEnumerable<TableName> GetTableNames(IDbConnection connection);
        IEnumerable<FieldInfo> GetFieldInfos(IDbConnection connection, string schema, string table);
    }
}