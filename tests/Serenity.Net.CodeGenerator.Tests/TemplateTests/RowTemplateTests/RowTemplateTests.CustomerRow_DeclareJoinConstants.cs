namespace Serenity.Tests.CodeGenerator;

public partial class RowTemplateTests
{
    const string ExpectedJoinConstantsTestRowCS =
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
                const string jCity = nameof(jCity);

                [DisplayName("Test Id"), Identity, IdProperty]
                public int? CustomerId
                {
                    get => fields.CustomerId[this];
                    set => fields.CustomerId[this] = value;
                }

                [DisplayName("Test Name"), Size(50), NotNull, QuickSearch, NameProperty]
                public string CustomerName
                {
                    get => fields.CustomerName[this];
                    set => fields.CustomerName[this] = value;
                }

                [DisplayName("City"), ForeignKey("[test].[City]", "CityId"), LeftJoin(jCity), TextualField("CityCityName")]
                public int? CityId
                {
                    get => fields.CityId[this];
                    set => fields.CityId[this] = value;
                }

                [DisplayName("City City Name"), Expression($"{jCity}.[CityName]")]
                public string CityCityName
                {
                    get => fields.CityCityName[this];
                    set => fields.CityCityName[this] = value;
                }

                [DisplayName("City Country Id"), Expression($"{jCity}.[CountryId]")]
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

                    public StringField CityCityName;
                    public Int32Field CityCountryId;
                }
            }
        }
        """"";

    [Fact]
    public void CustomerRow_DeclareJoinConstants()
    {
        var model = new MockEntityModel(joinConstants: true);
        var actual = RenderTemplate(model);
        AssertEqual(ExpectedJoinConstantsTestRowCS, actual);
    }
}
