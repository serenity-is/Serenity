namespace Serenity.CodeGenerator;

public partial class RetrieveHandlerTemplateTests : BaseTemplateTest
{
    protected override string TemplateName => "RetrieveHandler";

    const string Expected_Customer_Defaults =
        """""
        using Serenity.Services;

        namespace TestNamespace.TestModule
        {
            public interface ICustomerRetrieveHandler : IRetrieveHandler<CustomerRow, RetrieveRequest, RetrieveResponse<CustomerRow>> { }

            public class CustomerRetrieveHandler(IRequestContext context) :
                RetrieveRequestHandler<CustomerRow, RetrieveRequest, RetrieveResponse<CustomerRow>>(context),
                ICustomerRetrieveHandler
            {
            }
        }
        """"";

    [Fact]
    public void Customer_Defaults()
    {
        var model = new CustomerEntityModel
        {
        };
        var actual = RenderTemplate(model);
        AssertEqual(Expected_Customer_Defaults, actual);
    }
}
