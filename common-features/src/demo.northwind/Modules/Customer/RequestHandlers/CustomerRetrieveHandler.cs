using MyRow = Serenity.Demo.Northwind.CustomerRow;

namespace Serenity.Demo.Northwind;

public interface ICustomerRetrieveHandler : IRetrieveHandler<MyRow> { }

public class CustomerRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandler<MyRow>(context), ICustomerRetrieveHandler
{
}