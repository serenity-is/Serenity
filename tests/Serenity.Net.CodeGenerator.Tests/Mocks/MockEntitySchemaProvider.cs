
using Serenity.CodeGenerator;
using Serenity.Data.Schema;

namespace Serenity.Tests;

public class MockEntityDataSchema : IEntityDataSchema
{
    public string DefaultSchema { get; set; } = "dbo";

    public Func<string, string, IEnumerable<Data.Schema.FieldInfo>> OnGetFieldInfos;

    public IEnumerable<Data.Schema.FieldInfo> GetFieldInfos(string schema, string table)
    {
        return OnGetFieldInfos?.Invoke(schema, table) ?? Array.Empty<Data.Schema.FieldInfo>();
    }

    public Func<string, string, IEnumerable<ForeignKeyInfo>> OnGetForeignKeys;

    public IEnumerable<ForeignKeyInfo> GetForeignKeys(string schema, string table)
    {
        return OnGetForeignKeys?.Invoke(schema, table) ?? Array.Empty<ForeignKeyInfo>();
    }

    public Func<string, string, IEnumerable<string>> OnGetIdentityFields;

    public IEnumerable<string> GetIdentityFields(string schema, string table)
    {
        return OnGetIdentityFields?.Invoke(schema, table) ?? Array.Empty<string>();
    }

    public Func<string, string, IEnumerable<string>> OnGetPrimaryKeyFields;

    public IEnumerable<string> GetPrimaryKeyFields(string schema, string table)
    {
        return OnGetPrimaryKeyFields?.Invoke(schema, table) ?? Array.Empty<string>();
    }

    public Func<IEnumerable<TableName>> OnGetTableNames;

    public IEnumerable<TableName> GetTableNames()
    {
        return OnGetTableNames?.Invoke() ?? Array.Empty<TableName>();
    }
}