namespace Serenity.CodeGenerator;

public partial class DeleteHandlerTemplateTests : BaseTemplateTest
{
    protected override string TemplateName => "DeleteHandler";

    const string Expected_Customer_Defaults =
        """""
        using Serenity.Services;
        using MyRow = TestNamespace.TestModule.CustomerRow;

        namespace TestNamespace.TestModule
        {
            public interface ICustomerDeleteHandler : IDeleteHandler<MyRow, DeleteRequest, DeleteResponse> { }

            public class CustomerDeleteHandler(IRequestContext context) :
                DeleteRequestHandler<MyRow, DeleteRequest, DeleteResponse>(context),
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
