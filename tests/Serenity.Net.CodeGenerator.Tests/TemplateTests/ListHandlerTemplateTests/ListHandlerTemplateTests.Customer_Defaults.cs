namespace Serenity.CodeGenerator;

public partial class ListHandlerTemplateTests : BaseTemplateTest
{
    protected override string TemplateName => "ListHandler";

    const string Expected_Customer_Defaults =
        """""
        using Serenity.Services;
        using MyRow = TestNamespace.TestModule.CustomerRow;

        namespace TestNamespace.TestModule
        {
            public interface ICustomerListHandler : IListHandlerAsync<MyRow, ListRequest, ListResponse<MyRow>> { }

            public class CustomerListHandler(IRequestContext context) :
                ListRequestHandlerAsync<MyRow, ListRequest, ListResponse<MyRow>>(context),
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
