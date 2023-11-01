namespace Serenity.Tests.CodeGenerator;

public partial class RowTemplateTests
{
    const string Customer_Expected_EnableGenerateFields =
            """""
        using Serenity.ComponentModel;
        using Serenity.Data;
        using Serenity.Data.Mapping;
        using System.ComponentModel;

        namespace TestNamespace.TestModule
        {
            [ConnectionKey("TestConnection"), Module("TestModule"), TableName("[test].[Customer]")]
            [DisplayName("Customer"), InstanceName("Customer"), GenerateFields]
            [ReadPermission("TestPermission")]
            [ModifyPermission("TestPermission")]
            public sealed partial class CustomerRow : Row<CustomerRow.RowFields>, IIdRow, INameRow
            {
                [DisplayName("Customer Id"), Identity, IdProperty]
                public int? CustomerId { get => fields.CustomerId[this]; set => fields.CustomerId[this] = value; }

                [DisplayName("Customer Name"), Size(50), NotNull, QuickSearch, NameProperty]
                public string CustomerName { get => fields.CustomerName[this]; set => fields.CustomerName[this] = value; }

                [DisplayName("City"), ForeignKey("[test].[City]", "CityId"), LeftJoin("jCity"), TextualField(nameof(CityName))]
                public int? CityId { get => fields.CityId[this]; set => fields.CityId[this] = value; }

                [DisplayName("City Name"), Expression("jCity.[CityName]")]
                public string CityName { get => fields.CityName[this]; set => fields.CityName[this] = value; }

                [DisplayName("City Country Id"), Expression("jCity.[CountryId]")]
                public int? CityCountryId { get => fields.CityCountryId[this]; set => fields.CityCountryId[this] = value; }
            }
        }
        """"";
    [Fact]
    public void Customer_EnableGenerateFields()
    {
        var model = new CustomerEntityModel
        {
            EnableGenerateFields = true
        };
        var actual = RenderTemplate(model);

        AssertEqual(Customer_Expected_EnableGenerateFields, actual);
    }
}
