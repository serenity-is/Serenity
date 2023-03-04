using Serenity.CodeGenerator;
using static Serenity.Tests.MockModelInputs;

namespace Serenity.Tests;

public class MockEntityModel : EntityModel
{
    public MockEntityModel()
    {
        Module = TestModule;
        ConnectionKey = TestConnection;
        Permission = TestPermission;
        RootNamespace = TestNamespace;
        ClassName = Customer;
        RowClassName = Customer + "Row";
        Schema = TestSchema;
        Tablename = Customer;
        Title = Customer;
        Identity = "CustomerId";
        RowBaseClass = "Serenity.Data.Row<CustomerRow.RowFields>";
        Instance = false;
        NameField = "TestName";
        FieldPrefix = "";
        AspNetCore = true;
        NET5Plus = true;

        Fields.Add(new()
        {
            FieldType = "Int32",
            DataType = "int",
            TSType = "number",
            Ident = "CustomerId",
            Name = "CustomerId",
            Title = "Customer Id",
            FlagList = new() { new("Serenity.Data.Mapping.Identity") },
            Insertable = false,
            Updatable = false,
            IsValueType = true,
            Scale = 0,
            AttributeList = new()
            {
                new("System.ComponentModel.DisplayName", "\"Test Id\""),
                new("Serenity.Data.Mapping.Identity"),
                new("Serenity.Data.Mapping.IdProperty"),
            },
            ColAttributeList = new()
            {
                new("Serenity.ComponentModel.EditLink"),
                new("System.ComponentModel.DisplayName", "\"Db.Shared.RecordId\""),
                new("Serenity.ComponentModel.AlignRight")
            }
        });

        Fields.Add(new()
        {
            FieldType = "String",
            DataType = "string",
            TSType = "string",
            Ident = "CustomerName",
            Name = "CustomerName",
            Title = "Customer Name",
            FlagList = new() { new("Serenity.Data.Mapping.NotNull") },
            Insertable = false,
            Updatable = false,
            IsValueType = false,
            Size = 50,
            Scale = 0,
            AttributeList = new()
            {
                new("System.ComponentModel.DisplayName", "\"Test Name\""),
                new("Serenity.Data.Mapping.Size", "50"),
                new("Serenity.Data.Mapping.NotNull"),
                new("Serenity.Data.Mapping.QuickSearch"),
                new("Serenity.Data.NameProperty"),
            },
            ColAttributeList = new()
            {
                new("Serenity.ComponentModel.EditLink")
            }
        });

        Fields.Add(new()
        {
            FieldType = "Int32",
            DataType = "int",
            TSType = "number",
            Ident = "CityId",
            Name = "CityId",
            Title = "City",
            PKSchema = "test",
            PKTable = "City",
            PKColumn = "CityId",
            ForeignJoinAlias = "City",
            Insertable = false,
            Updatable = false,
            IsValueType = true,
            Scale = 0,
            TextualField = "CityCityName",
            AttributeList = new()
            {
                new("System.ComponentModel.DisplayName", "\"City\""),
                new("Serenity.Data.Mapping.ForeignKey", "\"[test].[City]\", \"CityId\""),
                new("Serenity.Data.Mapping.LeftJoin", "\"jCity\""),
                new("Serenity.Data.Mapping.TextualField", "\"CityCityName\"")
            },
        });

        Joins.Add(new()
        {
            Name = "City",
            SourceField = "CityId",
            Fields = new()
            {
                new()
                {
                    FieldType = "String",
                    DataType = "string",
                    TSType = "string",
                    Ident = "CityCityName",
                    Name = "CityName",
                    Title = "City City Name",
                    Insertable = false,
                    Updatable = false,
                    IsValueType = false,
                    Size = 50,
                    Scale = 0,
                    AttributeList = new()
                    {
                        new("System.ComponentModel.DisplayName", "\"City City Name\""),
                        new("Serenity.Data.Mapping.Expression", "\"jCity.[CityName]\"")
                    },
                    Expression = "jCity.[CityName]"
                },
                new()
                {
                    FieldType = "Int32",
                    DataType = "int",
                    TSType = "number",
                    Ident = "CityCountryId",
                    Name = "CountryId",
                    Title = "City Country Id",
                    Insertable = false,
                    Updatable = false,
                    IsValueType = true,
                    Scale = 0,
                    AttributeList = new()
                    {
                        new("System.ComponentModel.DisplayName", "\"City Country Id\""),
                        new("Serenity.Data.Mapping.Expression", "\"jCity.[CountryId]\"")
                    },
                    Expression = "jCity.[CountryId]"
                }
            }
        });
    }
}