namespace Serenity.Tests.CodeGenerator;

public partial class RowTemplateTests : BaseTemplateTest
{
    protected override string TemplateName => "Row";

    const string Customer_Expected_Defaults =
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
            public sealed class CustomerRow : Row<CustomerRow.RowFields>, IIdRow, INameRow
            {
                [DisplayName("Customer Id"), Identity, IdProperty]
                public int? CustomerId
                {
                    get => fields.CustomerId[this];
                    set => fields.CustomerId[this] = value;
                }

                [DisplayName("Customer Name"), Size(50), NotNull, QuickSearch, NameProperty]
                public string CustomerName
                {
                    get => fields.CustomerName[this];
                    set => fields.CustomerName[this] = value;
                }

                [DisplayName("City"), ForeignKey("[test].[City]", "CityId"), LeftJoin("jCity"), TextualField(nameof(CityName))]
                public int? CityId
                {
                    get => fields.CityId[this];
                    set => fields.CityId[this] = value;
                }

                [DisplayName("City Name"), Expression("jCity.[CityName]")]
                public string CityName
                {
                    get => fields.CityName[this];
                    set => fields.CityName[this] = value;
                }

                [DisplayName("City Country Id"), Expression("jCity.[CountryId]")]
                public int? CityCountryId
                {
                    get => fields.CityCountryId[this];
                    set => fields.CityCountryId[this] = value;
                }

                public class RowFields : RowFieldsBase
                {
                    public Int32Field CustomerId;
                    public StringField CustomerName;
                    public Int32Field CityId;

                    public StringField CityName;
                    public Int32Field CityCountryId;
                }
            }
        }
        """"";

    [Fact]
    public void Customer_Defaults()
    {
        var model = new CustomerEntityModel();
        var actual = RenderTemplate(model);
        AssertEqual(Customer_Expected_Defaults, actual);
    }
}
