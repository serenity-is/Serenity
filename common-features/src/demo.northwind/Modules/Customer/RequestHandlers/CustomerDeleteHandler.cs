using MyRow = Serenity.Demo.Northwind.CustomerRow;

namespace Serenity.Demo.Northwind;

public interface ICustomerDeleteHandler : IDeleteHandler<MyRow> { }

public class CustomerDeleteHandler(IRequestContext context) :
    DeleteRequestHandler<MyRow>(context), ICustomerDeleteHandler
{
}