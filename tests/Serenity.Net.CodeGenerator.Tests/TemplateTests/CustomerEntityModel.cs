using Serenity.CodeGenerator;
using static Serenity.Tests.CustomerEntityInputs;

namespace Serenity.Tests;

public class CustomerEntityModel : EntityModel
{
    public CustomerEntityModel(bool joinConstants = false)
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
        Identity = CustomerId;
        RowBaseClass = "Serenity.Data.Row<CustomerRow.RowFields>";
        NameField = CustomerName;
        FieldPrefix = "";
        AspNetCore = true;
        NET5Plus = true;
        DeclareJoinConstants = joinConstants;

        Fields.Add(new()
        {
            FieldType = "Int32",
            DataType = "int",
            TSType = "number",
            PropertyName = CustomerId,
            Name = CustomerId,
            Title = "Customer Id",
            FlagList = { new("Serenity.Data.Mapping.Identity") },
            IsValueType = true,
            OmitInForm = true,
            Scale = 0,
            AttributeList =
            {
                new("System.ComponentModel.DisplayName", "\"Customer Id\""),
                new("Serenity.Data.Mapping.Identity"),
                new("Serenity.Data.Mapping.IdProperty"),
            },
            ColAttributeList =
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
            PropertyName = CustomerName,
            Name = CustomerName,
            Title = "Customer Name",
            FlagList = { new("Serenity.Data.Mapping.NotNull") },
            IsValueType = false,
            Size = 50,
            Scale = 0,
            AttributeList =
            {
                new("System.ComponentModel.DisplayName", "\"Customer Name\""),
                new("Serenity.Data.Mapping.Size", "50"),
                new("Serenity.Data.Mapping.NotNull"),
                new("Serenity.Data.Mapping.QuickSearch"),
                new("Serenity.Data.NameProperty"),
            },
            ColAttributeList =
            {
                new("Serenity.ComponentModel.EditLink")
            }
        });

        Fields.Add(new()
        {
            FieldType = "Int32",
            DataType = "int",
            TSType = "number",
            PropertyName = CityId,
            Name = CityId,
            Title = City,
            PKSchema = TestSchema,
            PKTable = City,
            PKColumn = CityId,
            ForeignJoinAlias = "jCity",
            IsValueType = true,
            Scale = 0,
            TextualField = "CityName",
            AttributeList =
            {
                new("System.ComponentModel.DisplayName", "\"City\""),
                new("Serenity.Data.Mapping.ForeignKey", "\"[test].[City]\", \"CityId\""),
                new("Serenity.Data.Mapping.LeftJoin", joinConstants ? "jCity" : "\"jCity\""),
                new("Serenity.Data.Mapping.TextualField", "nameof(CityName)")
            },
        });

        Joins.Add(new()
        {
            Name = "City",
            SourceField = "CityId",
            Fields =
            {
                new()
                {
                    FieldType = "String",
                    DataType = "string",
                    TSType = "string",
                    PropertyName = CityName,
                    Name = CityName,
                    Title = "City Name",
                    IsValueType = false,
                    Size = 50,
                    Scale = 0,
                    AttributeList =
                    {
                        new("System.ComponentModel.DisplayName", "\"City Name\""),
                        new("Serenity.Data.Mapping.Expression", 
                            joinConstants ? "$\"{jCity}.[CityName]\"" : "\"jCity.[CityName]\"")
                    },
                    Expression = "jCity.[CityName]"
                },
                new()
                {
                    FieldType = "Int32",
                    DataType = "int",
                    TSType = "number",
                    PropertyName = "CityCountryId",
                    Name = CountryId,
                    Title = "City Country Id",
                    IsValueType = true,
                    Scale = 0,
                    AttributeList =
                    {
                        new("System.ComponentModel.DisplayName", "\"City Country Id\""),
                        new("Serenity.Data.Mapping.Expression", 
                            joinConstants ? "$\"{jCity}.[CountryId]\"" : "\"jCity.[CountryId]\"")
                    },
                    Expression = "jCity.[CountryId]"
                }
            }
        });
    }
}