using System.Data;
using MyRow = Serenity.Demo.Northwind.CustomerRow;

namespace Serenity.Demo.Northwind;

public interface ICustomerGetNextNumberHandler : IRequestHandler
{
    GetNextNumberResponse GetNextNumber(IDbConnection connection, GetNextNumberRequest request);
}

public class CustomerGetNextNumberHandler : ICustomerGetNextNumberHandler
{
    public GetNextNumberResponse GetNextNumber(IDbConnection connection, GetNextNumberRequest request)
    {
        return GetNextNumberHelper.GetNextNumber(connection, request, MyRow.Fields.CustomerID);
    }
}