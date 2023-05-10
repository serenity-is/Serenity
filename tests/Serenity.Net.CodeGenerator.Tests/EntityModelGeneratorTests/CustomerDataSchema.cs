using static Serenity.Tests.CustomerEntityInputs;
using Serenity.CodeGenerator;
using Serenity.Data.Schema;

namespace Serenity.Tests;

public class CustomerDataSchema : IEntityDataSchema
{
    public CustomerDataSchema()
    {
        FieldInfos.Add((TestSchema, Customer, new()
        {
            DataType = "int",
            FieldName = "CustomerId",
            IsIdentity = true,
            IsNullable = false,
            IsPrimaryKey = true
        }));

        FieldInfos.Add((TestSchema, Customer, new()
        {
            DataType = "nvarchar",
            FieldName = "CustomerName",
            IsNullable = false,
            Size = 50
        }));

        FieldInfos.Add((TestSchema, Customer, new()
        {
            DataType = "int",
            FieldName = "CityId",
            IsNullable = true,
            PKTable = "City",
            PKColumn = "CityId",
            PKSchema = TestSchema,
        }));

        FieldInfos.Add((TestSchema, City, new()
        {
            DataType = "nvarchar",
            FieldName = "CityName",
            IsNullable = false,
            Size = 50
        }));

        FieldInfos.Add((TestSchema, City, new()
        {
            DataType = "int",
            FieldName = "CountryId",
            IsNullable = false,
            PKSchema = TestSchema,
            PKTable = "Country",
            PKColumn = "CountryId"
        }));

        FieldInfos.Add((TestSchema, Country, new()
        {
            DataType = "int",
            FieldName = "CountryId",
            IsPrimaryKey = true,
            IsIdentity = true,
            IsNullable = false,
        }));

        FieldInfos.Add((TestSchema, Country, new()
        {
            DataType = "nvarchar",
            FieldName = "CountryName",
            IsNullable = false,
            Size = 50
        }));

        ForeignKeys.Add((TestSchema, Customer, new()
        {
            FKName = "FK_Customer_CityId",
            FKColumn = "CityId",
            PKSchema = TestSchema,
            PKTable = City,
            PKColumn = "CityId",
        }));

        ForeignKeys.Add((TestSchema, Customer, new()
        {
            FKName = "FK_City_CountryId",
            FKColumn = "CountryId",
            PKSchema = TestSchema,
            PKTable = Country,
            PKColumn = "CountryId"
        }));

    }

    public string DefaultSchema => "dbo";

    public List<(string schema, string table, Data.Schema.FieldInfo item)> FieldInfos { get; } = new();

    public IEnumerable<Data.Schema.FieldInfo> GetFieldInfos(string schema, string table)
    {
        return FieldInfos.Where(x => x.schema == schema && x.table == table)
            .Select(x => x.item);
    }

    public List<(string schema, string table, ForeignKeyInfo item)> ForeignKeys { get; } = new();

    public IEnumerable<ForeignKeyInfo> GetForeignKeys(string schema, string table)
    {
        return ForeignKeys.Where(x => x.schema == schema && x.table == table)
            .Select(x => x.item);
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
}