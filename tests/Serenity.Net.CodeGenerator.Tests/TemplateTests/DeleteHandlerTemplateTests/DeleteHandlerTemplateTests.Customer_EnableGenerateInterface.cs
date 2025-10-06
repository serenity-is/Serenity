namespace Serenity.CodeGenerator;

public partial class DeleteHandlerTemplateTests : BaseTemplateTest
{
    const string Expected_Customer_EnableGenerateInterface =
        """""
        using Serenity.Services;

        namespace TestNamespace.TestModule.RequestHandlers
        {
            [GenerateInterface]
            public class CustomerDeleteHandler(IRequestContext context) :
                DeleteRequestHandler<CustomerRow, DeleteRequest, DeleteResponse>(context),
                ICustomerDeleteHandler
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
