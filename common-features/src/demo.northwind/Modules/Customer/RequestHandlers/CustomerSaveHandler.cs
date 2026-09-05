using MyRow = Serenity.Demo.Northwind.CustomerRow;

namespace Serenity.Demo.Northwind;

public interface ICustomerSaveHandler : ISaveHandlerAsync<MyRow> { }

public class CustomerSaveHandler(IRequestContext context) :
    SaveRequestHandlerAsync<MyRow>(context), ICustomerSaveHandler
{
}