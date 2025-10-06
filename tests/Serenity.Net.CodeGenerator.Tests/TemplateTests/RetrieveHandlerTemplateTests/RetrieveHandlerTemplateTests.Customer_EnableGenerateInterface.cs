namespace Serenity.CodeGenerator;

public partial class RetrieveHandlerTemplateTests : BaseTemplateTest
{
    const string Expected_Customer_EnableGenerateInterface =
        """""
        using Serenity.Services;

        namespace TestNamespace.TestModule.RequestHandlers
        {
            [GenerateInterface]
            public class CustomerRetrieveHandler(IRequestContext context) :
                RetrieveRequestHandler<CustomerRow, RetrieveRequest, RetrieveResponse<CustomerRow>>(context),
                ICustomerRetrieveHandler
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
