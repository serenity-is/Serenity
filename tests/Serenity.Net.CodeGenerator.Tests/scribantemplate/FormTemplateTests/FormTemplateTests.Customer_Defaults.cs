namespace Serenity.CodeGenerator;

public partial class FormTemplateTests : BaseTemplateTest
{
    protected override string TemplateName => "Form";

    const string Customer_ExpectedDefaults =
        """""
        using Serenity.ComponentModel;

        namespace TestNamespace.TestModule.Forms
        {
            [FormScript("TestModule.Customer")]
            [BasedOnRow(typeof(CustomerRow), CheckNames = true)]
            public class CustomerForm
            {
                public string CustomerName { get; set; }
                public int? CityId { get; set; }
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

    const string Customer_ExpectedNullableRefTypes =
        """""
        using Serenity.ComponentModel;

        namespace TestNamespace.TestModule.Forms
        {
            [FormScript("TestModule.Customer")]
            [BasedOnRow(typeof(CustomerRow), CheckNames = true)]
            public class CustomerForm
            {
                public string? CustomerName { get; set; }
                public int? CityId { get; set; }
            }
        }
        """"";

    [Fact]
    public void Customer_NullableRefTypes()
    {
        var model = new CustomerEntityModel(nullableRefTypes: true);
        var actual = RenderTemplate(model);
        AssertEqual(Customer_ExpectedNullableRefTypes, actual);
    }
}
