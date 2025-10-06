namespace Serenity.CodeGenerator;

public partial class DeleteHandlerTemplateTests : BaseTemplateTest
{
    protected override string TemplateName => "DeleteHandler";

    const string Expected_Customer_Defaults =
        """""
        using Serenity.Services;

        namespace TestNamespace.TestModule
        {
            public interface ICustomerDeleteHandler : IDeleteHandler<CustomerRow, DeleteRequest, DeleteResponse> { }

            public class CustomerDeleteHandler(IRequestContext context) :
                DeleteRequestHandler<CustomerRow, DeleteRequest, DeleteResponse>(context),
                ICustomerDeleteHandler
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
