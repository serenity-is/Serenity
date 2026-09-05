using MyRow = Serenity.Demo.Northwind.CustomerRow;

namespace Serenity.Demo.Northwind;

public interface ICustomerDeleteHandler : IDeleteHandlerAsync<MyRow> { }

public class CustomerDeleteHandler(IRequestContext context) :
    DeleteRequestHandlerAsync<MyRow>(context), ICustomerDeleteHandler
{
}