namespace Serenity.CodeGenerator;

public partial class SaveHandlerTemplateTests : BaseTemplateTest
{
    const string Expected_Customer_EnableGenerateInterface =
        """""
        using Serenity.Services;

        namespace TestNamespace.TestModule.RequestHandlers
        {
            [GenerateInterface]
            public class CustomerSaveHandler(IRequestContext context) :
                SaveRequestHandler<CustomerRow, SaveRequest<CustomerRow>, SaveResponse>(context),
                ICustomerSaveHandler
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
