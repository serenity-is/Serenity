namespace Serenity.CodeGenerator;

public partial class ListHandlerTemplateTests : BaseTemplateTest
{
    protected override string TemplateName => "ListHandler";

    const string Expected_Customer_Defaults =
        """""
        using Serenity.Services;

        namespace TestNamespace.TestModule
        {
            public interface ICustomerListHandler : IListHandler<CustomerRow, ListRequest, ListResponse<CustomerRow>> { }

            public class CustomerListHandler(IRequestContext context) :
                ListRequestHandler<CustomerRow, ListRequest, ListResponse<CustomerRow>>(context),
                ICustomerListHandler
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
