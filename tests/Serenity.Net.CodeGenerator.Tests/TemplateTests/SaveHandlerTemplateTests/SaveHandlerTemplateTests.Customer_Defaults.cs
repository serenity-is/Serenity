namespace Serenity.CodeGenerator;

public partial class SaveHandlerTemplateTests : BaseTemplateTest
{
    protected override string TemplateName => "SaveHandler";

    const string Expected_Customer_Defaults =
        """""
        using Serenity.Services;
        using MyRow = TestNamespace.TestModule.CustomerRow;

        namespace TestNamespace.TestModule
        {
            public interface ICustomerSaveHandler : ISaveHandler<MyRow, SaveRequest<MyRow>, SaveResponse> { }

            public class CustomerSaveHandler(IRequestContext context) :
                SaveRequestHandler<MyRow, SaveRequest<MyRow>, SaveResponse>(context),
                ICustomerSaveHandler
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
