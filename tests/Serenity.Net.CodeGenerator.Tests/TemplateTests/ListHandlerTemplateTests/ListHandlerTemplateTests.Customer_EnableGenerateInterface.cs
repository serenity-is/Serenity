namespace Serenity.CodeGenerator;

public partial class ListHandlerTemplateTests : BaseTemplateTest
{
    const string Expected_Customer_EnableGenerateInterface =
        """""
        using Serenity.Services;

        namespace TestNamespace.TestModule.RequestHandlers
        {
            [GenerateInterface]
            public class CustomerListHandler(IRequestContext context) :
                ListRequestHandler<CustomerRow, ListRequest, ListResponse<CustomerRow>>(context),
                ICustomerListHandler
            {
            }
        }
        """"";

    [Fact]
    public void Customer_EnableGenerateInterface()
    {
        var model = new CustomerEntityModel
        {
            EnableGenerateInterface = true
        };
        var actual = RenderTemplate(model);
        AssertEqual(Expected_Customer_EnableGenerateInterface, actual);
    }
}
