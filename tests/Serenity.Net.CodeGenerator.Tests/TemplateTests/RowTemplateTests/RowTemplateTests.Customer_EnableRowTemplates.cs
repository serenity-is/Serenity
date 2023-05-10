namespace Serenity.Tests.CodeGenerator;

public partial class RowTemplateTests
{
    const string Customer_Expected_EnableRowTemplates =
            """""
        using Serenity.ComponentModel;
        using Serenity.Data;
        using Serenity.Data.Mapping;
        using System.ComponentModel;

        namespace TestNamespace.TestModule
        {
            [ConnectionKey("TestConnection"), Module("TestModule"), TableName("[test].[Customer]")]
            [DisplayName("Customer"), InstanceName("Customer")]
            [ReadPermission("TestPermission")]
            [ModifyPermission("TestPermission")]
            public sealed partial class CustomerRow : Row<CustomerRow.RowFields>, IIdRow, INameRow
            {
                class RowTemplate
                {
                    [DisplayName("Customer Id"), Identity, IdProperty]
                    public int? CustomerId { get; set; }

                    [DisplayName("Customer Name"), Size(50), NotNull, QuickSearch, NameProperty]
                    public string CustomerName { get; set; }

                    [DisplayName("City"), ForeignKey("[test].[City]", "CityId"), LeftJoin("jCity"), TextualField(nameof(CityName))]
                    public int? CityId { get; set; }

                    [DisplayName("City Name"), Expression("jCity.[CityName]")]
                    public string CityName { get; set; }

                    [DisplayName("City Country Id"), Expression("jCity.[CountryId]")]
                    public int? CityCountryId { get; set; }
                }
            }
        }
        """"";
    [Fact]
    public void Customer_EnableRowTemplates()
    {
        var model = new CustomerEntityModel
        {
           EnableRowTemplates = true
        };
        var actual = RenderTemplate(model);
        
        AssertEqual(Customer_Expected_EnableRowTemplates, actual);
    }
}
