using MyRow = Serenity.Demo.Northwind.CustomerRow;

namespace Serenity.Demo.Northwind;

public interface ICustomerRetrieveHandler : IRetrieveHandlerAsync<MyRow> { }

public class CustomerRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandlerAsync<MyRow>(context), ICustomerRetrieveHandler
{
}