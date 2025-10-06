using MyRow = Serenity.Demo.Northwind.CustomerRow;

namespace Serenity.Demo.Northwind;

public interface ICustomerListHandler : IListHandler<MyRow> { }

public class CustomerListHandler(IRequestContext context) :
    ListRequestHandler<MyRow>(context), ICustomerListHandler
{
}