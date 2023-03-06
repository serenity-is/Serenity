namespace Serenity.Tests.CodeGenerator;

public partial class ColumnsTemplateTests : BaseTemplateTest
{
    protected override string TemplateName => "Columns";

    const string Customer_ExpectedDefaults =
        """""
        using Serenity.ComponentModel;
        using System.ComponentModel;

        namespace TestNamespace.TestModule.Columns
        {
            [ColumnsScript("TestModule.Customer")]
            [BasedOnRow(typeof(CustomerRow), CheckNames = true)]
            public class CustomerColumns
            {
                [EditLink, DisplayName("Db.Shared.RecordId"), AlignRight]
                public int CustomerId { get; set; }
                [EditLink]
                public string CustomerName { get; set; }
                public string CityName { get; set; }
            }
        }
        """"";

    [Fact]
    public void Customer_Defaults()
    {
        var model = new CustomerEntityModel();
        var actual = RenderTemplate(model);
        AssertEqual(Customer_ExpectedDefaults, actual);
    }
}
