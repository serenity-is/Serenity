using MyRow = Serenity.Demo.Northwind.CustomerRow;

namespace Serenity.Demo.Northwind;

public interface ICustomerListHandler : IListHandlerAsync<MyRow> { }

public class CustomerListHandler(IRequestContext context) :
    ListRequestHandlerAsync<MyRow>(context), ICustomerListHandler
{
}